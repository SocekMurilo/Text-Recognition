import base64
import numpy as np
import cv2
from tensorflow.keras import models

from joblib import dump, load

from flask import (
    Blueprint, flash, g, redirect, render_template, request, session, url_for,
    jsonify
)

bp = Blueprint('json', __name__, url_prefix='/json')


model = models.load_model("../checkpoints/testing/test_model8_93-91.keras")

def base64_to_image(base64_string, target_shape=(1200, 900)):
    # Decode base64 string into bytes
    image_bytes = base64.b64decode(base64_string)
    
    # Convert bytes to NumPy array
    nparr = np.frombuffer(image_bytes, np.uint8)
    
    # Decode the array into an image
    image = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
    
    # Resize the image to the target shape
    resized_image = cv2.resize(image, target_shape[:2])
    
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

@bp.route('/', methods=('GET', 'POST'))
def requisition():

    data = request.get_json()

    print(data)
    # img = base64_to_image(data['Image'])

    # img = np.expand_dims(img, axis=0)
    
    # # print(img)
    # a = model.predict(img)
    # print(get_destination_folder_name(np.argmax(a)))

    # DTC = load('../models/DTC.pkl')
    # lr = load('../models/LogisticRegression.pkl')
    # vectonizer = load('../models/vectonizer')

    # article = data['text']
    # a = tokenizer(article)
    # b = [a]
    # b = vectonizer.transform(b)

    # pred = DTC.predict(b)
    # predicted = { 'predicted': int(pred[0]) }

    return jsonify("Poggers")