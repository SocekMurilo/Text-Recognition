import cv2 as cv
import os
import numpy as np
import matplotlib.pyplot as plt

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

def resize_images_in_folder(Images, new_size):
    datas = []
    for folder in os.listdir(Images):
        folder = []
        for img in folder:
            # Resize the image using OpenCV
            resized_img = cv.resize(img, new_size)
            # Save the resized image, overwriting the original
            folder.append(resized_img)
        datas.append(folder)
    
    return datas

def resize_images_in_folder2(folder_path, new_size):
    datas = []
    for folder_name in os.listdir(folder_path):
        folder = []
        folder_full_path = os.path.join(folder_path, folder_name)
        for filename in os.listdir(folder_full_path):
            if filename.endswith('.jpg') or filename.endswith('.png') or filename.endswith('.jpeg'):
                img_path = os.path.join(folder_full_path, filename)
                # Open the image file using OpenCV
                img = cv.imread(img_path)
                # Resize the image using OpenCV
                resized_img = cv.resize(img, new_size)
                # Save the resized image, overwriting the original
                folder.append(resized_img)
        datas.append(folder)
    
    return datas

folders = count_folders("organizedData")

resized_data = resize_images_in_folder2("organizedData", (128, 128))

# print(resized_data)

for i in range(folders):
    # Create a folder for the current index if it doesn't exist
    folder_path = f"resizedData\{i}"
    os.makedirs(folder_path, exist_ok=True)

    # Loop through each binary image in limiarized[i]
    for img_index, img in enumerate(resized_data[i]):
        # Get the original image name
        img_name = f"{img_index}"  # Update this according to your file naming convention

        # Save the binary image with the original name in the corresponding folder
        save_path = os.path.join(folder_path, f"{img_name}.png")
        cv.imwrite(save_path, img)

