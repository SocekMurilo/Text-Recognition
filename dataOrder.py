import os
import shutil

def get_destination_folder_name(category):
    if category >= 0 and category <= 9:
        return str(category)
    elif category >= 10 and category <= 35:
        return chr(category + 55)  # Convert to ASCII character A-Z
    elif category >= 36 and category <= 61:
        return chr(category + 61)  # Convert to ASCII character a-z
    else:
        raise ValueError("Invalid category number")

def move_images(source_folder, destination_folder):
    # Get all files in the source folder
    files = os.listdir(source_folder)
    
    # Iterate through each file
    for file_name in files:
        # Check if the file is an image with the specified format
        if file_name.startswith('img') and file_name.endswith('.png'):
            # Extract the category number from the filename
            category = int(file_name.split('-')[0][3:])
            # Decrement the category number by 1
            category -= 1
            
            # Create the full path of the source file
            source_path = os.path.join(source_folder, file_name)
            
            # Create the full path of the destination folder for this category
            category_folder = os.path.join(destination_folder, str(category))
            
            # Create the destination folder if it doesn't exist
            if not os.path.exists(category_folder):
                os.makedirs(category_folder)
            
            # Create the full path of the destination file
            destination_path = os.path.join(category_folder, file_name)
            
            # Move the file to the destination folder
            shutil.move(source_path, destination_path)
            print(f"Moved {file_name} to {category_folder}")

# Example usage
source_folder = './data/Img'
destination_folder = './organizedData'
move_images(source_folder, destination_folder)