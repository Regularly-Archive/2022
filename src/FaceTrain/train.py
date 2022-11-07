import cv2
import os
import numpy as np
from PIL import Image
import json
import shutil
import dlib

face_landmark_path = './data/data_dlib/shape_predictor_68_face_landmarks.dat'
dlib_detector = dlib.get_frontal_face_detector()
dlib_predictor = dlib.shape_predictor(face_landmark_path)

recognizer = cv2.face.LBPHFaceRecognizer_create()
detector = cv2.CascadeClassifier("C:\\Users\\XA-162\\AppData\\Roaming\\Python\\Python37\\site-packages\\cv2\\data\\haarcascade_frontalface_alt.xml")

if os.path.exists('./train/datasets/'):
    shutil.rmtree('./train/datasets/')

os.makedirs('./train/datasets/')

def process(path):
    path = os.path.abspath(path)
    children = [os.path.join(path, f) for f in os.listdir(path)]
    children = list(filter(lambda x:os.path.isdir(x), children))
    labels = list(map(lambda x:os.path.split(x)[-1], children))
    for index in range(0, len(children)):
        child = children[index]
        image_paths = [os.path.join(child, f) for f in os.listdir(child)]
        image_paths = list(filter(lambda x:os.path.isfile(x), image_paths))
        number = 0
        for image_path in image_paths:
            if os.path.split(image_path)[-1].split(".")[-1] != "jpg":
                continue
            
            image = Image.open(image_path)
            image = cv2.cvtColor(np.asarray(image), cv2.COLOR_RGB2BGR)

            number += 1
            
            face_rects = dlib_detector(image, 0)
            for _, face in enumerate(face_rects):
                
                try:
                    x, y, w, h = face.left(), face.top(), face.right() - face.left(), face.bottom() - face.top()
                    cv2.imwrite(f'./train/datasets/Sample_{index}_{number}.jpg', image[y : y + h, x : x + w])
                except:
                    pass
            print(f'已完成对图片 {image_path} 的预处理')
    return labels


def get_images_and_labels(path):
    path = os.path.abspath(path)
    face_samples = []
    samples = [os.path.join(path, f) for f in os.listdir(path)]
    # samples = list(filter(lambda x:os.path.isfile(x), samples))
    for index in range(0, len(samples)):
        sample = samples[index]
        sampleId = int(os.path.split(sample)[-1].split('_')[1])
        if os.path.split(sample)[-1].split(".")[-1] != "jpg":
            continue
            
        image = Image.open(sample).convert('L')
        image_np = np.array(image, 'uint8')

        faces = detector.detectMultiScale(image_np, scaleFactor=1.2, minNeighbors=5)
        if len(faces) == 0:
            continue

        for x,y,w,h in faces:
            face_samples.append((image_np[y : y + h, x : x + w], sampleId))

    return face_samples

face_labels = process('./faces/')
with open('./train/face_map.json', 'wt', encoding='utf-8') as fw:
    json.dump(face_labels, fw)
face_samples = get_images_and_labels('./train/datasets/')
faces = list(map(lambda x:x[0], face_samples))
faceIds = list(map(lambda x:x[1], face_samples))
recognizer.train(faces, np.array(faceIds))
recognizer.save("./train/train_data.yml")





