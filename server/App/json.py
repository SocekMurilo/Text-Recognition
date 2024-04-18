import base64
import numpy as np
import cv2 as cv
from tensorflow.keras import models
import matplotlib.pyplot as plt
import os

from joblib import dump, load

from flask import (
    Blueprint, flash, g, redirect, render_template, request, session, url_for,
    jsonify
)

bp = Blueprint('json', __name__, url_prefix='/json')


model = models.load_model("../checkpoints/testing/test_model6_94-90.keras")

def base64_to_image(base64_string, target_shape=(1200, 900)):
    # Decode base64 string into bytes
    image_bytes = base64.b64decode(base64_string)
    
    # Convert bytes to NumPy array
    nparr = np.frombuffer(image_bytes, np.uint8)
    
    # Decode the array into an image
    image = cv.imdecode(nparr, cv.IMREAD_COLOR)
    
    # Resize the image to the target shape
    resized_image = cv.resize(image, target_shape[:2])
    
    print(resized_image.shape)

    return resized_image

def get_destination_folder_name(category):
    if category >= 0 and category <= 9:
        return str(category)
    elif category >= 10 and category <= 35:
        return chr(category + 55)  # Convert to ASCII character A-Z
    elif category >= 36 and category <= 61:
        return chr(category + 61)  # Convert to ASCII character a-z
    else:
        raise ValueError("Invalid category number")
    
def load_images_from_folder(folder):
    images = []
    for filename in os.listdir(folder):
        img = cv.imread(os.path.join(folder,filename))
        if img is not None:
            images.append(img)
    return images

def count_folders(directory):
    # Initialize a counter for folders
    folder_count = 0
    
    # Iterate over items in the directory
    for item in os.listdir(directory):
        # Check if the item is a directory
        if os.path.isdir(os.path.join(directory, item)):
            folder_count += 1
    
    return folder_count

def show(img):
    plt.imshow(img, cmap='gray')
    plt.show()
    return img



@bp.route('/', methods=('GET', 'POST'))
def requisition():

    data = request.get_json()
    path = "../Winforms/Words/"
    print(os.getcwd())
    folders = count_folders(path)

    data = []

    for i in range(folders):
        imgs = load_images_from_folder(f'{path}/Word_{i}/')
        data.append(imgs)

    str_result = ''

    for word in data:
        str_result = str_result + ' '
        for letter in word:
            # letter = cv.resize(letter, (1200, 900))
            c = np.expand_dims(letter, axis=0)
            
            a = model.predict(c)
            b = get_destination_folder_name(np.argmax(a))
            str_result = str_result + b

    # print(data)
    # img = base64_to_image(data['Image'])

    # img = np.expand_dims(img, axis=0)
    
    # # print(img)
    # a = model.predict(img)
    # print(get_destination_folder_name(np.argmax(a)))

    return jsonify(str_result)
    return jsonify("str_result")
