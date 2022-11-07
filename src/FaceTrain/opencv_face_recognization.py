import cv2
import os
import numpy as np
from PIL import Image
import json
import shutil
import logging

# Logging
logger = logging.getLogger(__name__)
logging.basicConfig(level=logging.INFO)

# 常量定义
FACES_DIR = './faces/'
DATASETS_DIR = './data/datasets/'
FACES_TRAIN_MODEL_FILE = './train/train_data.yml'
FACES_NAME_MAP_FILE = './data/face_map.json'

# OpenCV 
recognizer = cv2.face.LBPHFaceRecognizer_create()
detector = cv2.CascadeClassifier("C:\\Users\\XA-162\\AppData\\Roaming\\Python\\Python37\\site-packages\\cv2\\data\\haarcascade_frontalface_alt2.xml")

def pre_process_images(path):
    path = os.path.abspath(path)
    subDirs = [os.path.join(path, f) for f in os.listdir(path)]
    subDirs = list(filter(lambda x:os.path.isdir(x), subDirs))
    for index in range(0, len(subDirs)):
        number = 0
        subDir = subDirs[index]
        person_label = os.path.split(subDir)
        image_paths = [os.path.join(subDir, f) for f in os.listdir(subDir)]
        image_paths = list(filter(lambda x:os.path.isfile(x), image_paths))
        for image_path in image_paths:
            if os.path.split(image_path)[-1].split(".")[-1] != "jpg":
                continue
            
            image = Image.open(image_path)
            image = cv2.cvtColor(np.asarray(image), cv2.COLOR_RGB2BGR)
            image_gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
            # image_gray = cv2.equalizeHist(image_gray)
            faces = detector.detectMultiScale(image_gray)

            if len(faces) == 0:
                logger.warning(f'The image {image_path} can not detect any face.')
                continue

            for (x,y,w,h) in faces:
                cv2.imwrite(f"./data/datasets/{person_label}_{index}_{number}.jpg", image_gray[y : y + h, x : x + w])
        
        yield person_label


def get_images_and_labels(datasets_path):
    face_samples = []
    datasets_path = os.path.abspath(datasets_path)
    samples = [os.path.join(datasets_path, f) for f in os.listdir(datasets_path)]
    for index in range(0, len(samples)):
        sample = samples[index]
        sampleId = int(os.path.split(sample)[-1].split('_')[1])
    
        image = Image.open(sample).convert('L')
        image_np = np.array(image, 'uint8')

        faces = detector.detectMultiScale(image_np, scaleFactor=1.2, minNeighbors=5)
        if len(faces) == 0:
            continue

        for x,y,w,h in faces:
            face_samples.append((image_np[y : y + h, x : x + w], sampleId))

    return face_samples

def opencv_train_face_model():
    face_labels = list(pre_process_images(FACES_DIR))

    with open(FACES_NAME_MAP_FILE, 'wt', encoding='utf-8') as fw:
        json.dump(face_labels, fw)

    face_samples = get_images_and_labels(DATASETS_DIR)
    faces = list(map(lambda x:x[0], face_samples))
    faceIds = list(map(lambda x:x[1], face_samples))
    recognizer.train(faces, np.array(faceIds))
    recognizer.save(FACES_TRAIN_MODEL_FILE)


opencv_train_face_model()


