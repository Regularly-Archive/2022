import dlib
import cv2
import os
import math
import numpy as np
from PIL import Image, ImageEnhance
import imutils
from imutils import face_utils

face_landmark_path = 'shape_predictor_68_face_landmarks.dat'

detector = dlib.get_frontal_face_detector()
predictor = dlib.shape_predictor(face_landmark_path)

def face_recognition(img):
    facemarks = []
    face_rects = detector(img, 0)
    for index, face in enumerate(face_rects):
        shape = predictor(img, face_rects[index])
        facemarks.append((shape, face))
    return facemarks

def create_face_mask(image, facemark):
    shape, face = facemark
    shape2np = face_utils.shape_to_np(shape)
    mask = np.zeros(image.shape, dtype=np.uint8)
    points = np.concatenate([shape2np[0:16], shape2np[26:17:-1]])
    cv2.fillPoly(img=mask, pts=[points], color=(255,) * image.shape[2])
    return mask

def create_white_image(image):
    white = np.zeros(image.shape, dtype=np.uint8)
    for i in range(0, image.shape[0]):
        for j in range(0, image.shape[1]):
            white[ i, j ] = np.uint8(255)

    return white

def merge_face_mask(image, mask):
    return cv2.bitwise_and(image, mask)

def get_facemark_rect(shape):

    shape2np = face_utils.shape_to_np(shape)
    (x0, y0) = shape2np[0]

    minX = maxX = x0
    minY = maxY = y0

    for (x,y) in shape2np:
        if x < minX:
            minX = x
        if x >= maxX:
            maxX = x
        if y < minY:
            minY = y
        if y >= maxY:
            maxY = y

    return (minX, minY, maxX - minX, maxY - minY)

def remove_background(image):
    height, width = image.shape
    blur_img = image
    blur_img = cv2.floodFill(blur_img,mask=None,seedPoint=(0,0),newVal=(255,255,255))[1]
    blur_img = cv2.floodFill(blur_img, mask=None, seedPoint=(0,height-1), newVal=(255, 255, 255))[1]
    blur_img = cv2.floodFill(blur_img, mask=None, seedPoint=(width-1, height-1), newVal=(255, 255, 255))[1]
    blur_img = cv2.floodFill(blur_img, mask=None, seedPoint=(width-1, 0), newVal=(255, 255, 255))[1]
    return blur_img

def main():
    image = cv2.imread("./faces/face13.jpg")
    try:
         image.shape
    except:
        print("can not read image")
        return 

    image = imutils.resize(image, width=640,height=480) 
    facemarks = face_recognition(image)
    
    shape, face = facemarks[0]
    (x, y, w, h) = get_facemark_rect(shape)

    mask = create_face_mask(image, facemarks[0])
    image = merge_face_mask(image, mask)
    
    image = image[y:y + h, x:x + w]

    white = create_white_image(image)
    for i in range(0, image.shape[0]):
        for j in range(0, image.shape[1]):
            if ((image[i, j] != 0).all()):
                white[i, j] = image[i, j]
    
    image = white

   
    
    image = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    _, image = cv2.threshold(image, 95, 255, cv2.THRESH_BINARY)

    # image2 = cv2.adaptiveThreshold(image, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY,9,7)
    # cv2.imshow('image2', image2)

    # image3 = cv2.adaptiveThreshold(image, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY,9,7)
    # cv2.imshow('image3', image3)

    
    cv2.imshow("image", image)
    cv2.imwrite('face.jpg', image)
    
    bg = Image.open(r"PandaMan.jpg")
    face = Image.open(r"face.jpg")

    w_g, h_g = bg.size
    w_f, h_f = face.size

    ratio = h_f / w_f

    w_new = int(w_g * 0.4)
    h_new = int(ratio * w_new)

    resized = face.resize((w_new, h_new),Image.ANTIALIAS)

    left = int(w_g / 2 - w_new / 2)
    top = int(h_g / 2 - h_new / 2)
    
    bg.paste(resized, (left, top, left + w_new, top + h_new))
    bg.save('output.jpg')

    cv2.waitKey(0)
    
if __name__ == '__main__':
    main()
