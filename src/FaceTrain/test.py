import cv2
import os
import numpy as np
from PIL import Image, ImageDraw, ImageFont
import json
import uuid

font = ImageFont.truetype(font='msyh.ttc', size=36)
recognizer = cv2.face.LBPHFaceRecognizer_create()
recognizer.read('./train/train_data.yml')
detector = cv2.CascadeClassifier("C:\\Users\\XA-162\\AppData\\Roaming\\Python\\Python37\\site-packages\\cv2\\data\\haarcascade_frontalface_alt.xml")
names = []
with open('./train/face_map.json', 'rt', encoding='utf-8') as fr:
    names = json.load(fr)

def test(path):
    image = Image.open(path)
    image_np = np.array(image.convert('L'), 'uint8')
    image = cv2.cvtColor(np.asarray(image), cv2.COLOR_RGB2BGR)
 
    faces = detector.detectMultiScale(
        image_np
    )

    for (x, y, w, h) in faces:
        cv2.rectangle(image, (x, y), (x+w, y+h), (0, 255, 0), 1)
        idnum, confidence = recognizer.predict(image_np[y:y+h, x:x+w])
 
        if confidence > 50:
            idnumText = names[idnum]
            confidenceText = "{0}%".format(round(confidence))
        
        else:
            idnumText = "陌生人"
            confidenceText = "{0}%".format(round(confidence))
 
        image = cv2AddChineseText(image , f'{str(idnumText)},{str(confidenceText)}', (x + 5, y - 35),(255, 0, 0), 30)
        # print(f'{path}, {idnumText}, {confidenceText}')

        if confidence > 50:
            cv2.imwrite(f'./test/{str(uuid.uuid4())}.jpg', image)
        
        yield (idnum, confidence)

            
    # cv2.imshow('camera', image)
    # cv2.waitKey()

def cv2AddChineseText(img, text, position, textColor=(0, 255, 0), textSize=30):
    if (isinstance(img, np.ndarray)):  # 判断是否OpenCV图片类型
        img = Image.fromarray(cv2.cvtColor(img, cv2.COLOR_BGR2RGB))
    # 创建一个可以在给定图像上绘图的对象
    draw = ImageDraw.Draw(img)
    # 字体的格式
    fontStyle = ImageFont.truetype(
        "simsun.ttc", textSize, encoding="utf-8")
    # 绘制文本
    draw.text(position, text, textColor, font=fontStyle)
    # 转换回OpenCV格式
    return cv2.cvtColor(np.asarray(img), cv2.COLOR_RGB2BGR)


def main(path):
    total = 0
    success = 0
    path = os.path.abspath(path)
    children = [os.path.join(path, f) for f in os.listdir(path)]
    children = list(filter(lambda x:os.path.isdir(x), children))
    for index in range(0, len(children)):
        child = children[index]
        image_paths = [os.path.join(child, f) for f in os.listdir(child)]
        image_paths = list(filter(lambda x:os.path.isfile(x), image_paths))
        for image_path in image_paths:
            total += 1
            result = list(test(image_path))
            sorted(result, key=lambda x:x[1], reverse=True)
            if len(result) == 0:
                print(f'{image_path} 未识别出')
                continue
            predictedName = names[result[0][0]]
            actualName = os.path.split(child)[-1]
            print(f'{image_path}, 预测值：{predictedName}, 真实值：{actualName}，置信度：{result[0][1]}%')
            if actualName == predictedName:
                success += 1

    print(f'正确率：{success / total * 100}%')



main('./faces/')