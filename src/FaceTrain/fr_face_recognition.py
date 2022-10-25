import face_recognition
from sklearn import svm
from sklearn.metrics import accuracy_score
import joblib
import os
import logging
import numpy as np

# Logging
logger = logging.getLogger(__name__)
logging.basicConfig(level=logging.INFO)

# 常量定义
FACES_DIR = './faces/'
FACES_FEATURES_MODEL_FILE = './data/train_data.m'

def face_recognize_train(dir):

    face_encodings_mean_list = []

    path = os.path.abspath(dir)
    subDirs = [os.path.join(path, f) for f in os.listdir(path)]
    subDirs= list(filter(lambda x:os.path.isdir(x), subDirs))
    for subDir in subDirs:
        person_label = os.path.split(subDir)[-1]
        image_paths = [os.path.join(subDir, f) for f in os.listdir(subDir)]
        image_paths = list(filter(lambda x:os.path.isfile(x), image_paths))

        # 第 X 个人脸的特征集合
        face_encodings_person_x = []

        for image_path in image_paths:
            # 提取第 X 个人脸特征
            face = face_recognition.load_image_file(image_path)
            face_bounding_boxes = face_recognition.face_locations(face)
  
            if len(face_bounding_boxes) == 1:
                face_encodings = face_recognition.face_encodings(face)[0]
                face_encodings_person_x.append(face_encodings)
            else:
                logger.warning(person_label + "/" + image_path + " can't be used for training")
        
        # 计算第 X 个人脸特征的平均值 
        face_encoding_mean_x = np.zeros(128, dtype=object, order='C')
        if face_encodings_person_x :
            face_encoding_mean_x = np.array(face_encodings_person_x, dtype=object).mean(axis=0)

        face_encodings_mean_list.append((face_encoding_mean_x, person_label))

    # 使用支持向量机进行模型训练
    clf = svm.SVC(gamma ='scale')
    encodings = list(map(lambda x:x[0], face_encodings_mean_list))
    names = list(map(lambda x:x[1], face_encodings_mean_list))
    clf.fit(encodings, names)
    score = clf.score(encodings, names)
    logger.info(f"train score: {score}")
    return clf

def face_recoginize_test(clf, dir):
    total_images = 0
    matched_images = 0

    path = os.path.abspath(dir)
    subDirs = [os.path.join(path, f) for f in os.listdir(path)]
    subDirs= list(filter(lambda x:os.path.isdir(x), subDirs))
    for subDir in subDirs:
        person_label = os.path.split(subDir)[-1]
        image_paths = [os.path.join(subDir, f) for f in os.listdir(subDir)]
        image_paths = list(filter(lambda x:os.path.isfile(x), image_paths))
        total_images += len(image_paths)
        for image_path in image_paths:
            test_image = face_recognition.load_image_file(image_path)
            face_locations = face_recognition.face_locations(test_image)
            length = len(face_locations)
            
            # Predict all the faces in the test image using the trained classifier
            logger.info(f"{length} faces detected in {image_path}.")
            for i in range(length):
                test_image_enc = face_recognition.face_encodings(test_image)[i]
                predict = clf.predict([test_image_enc])
                score = accuracy_score([person_label], predict)
                logger.info(f"Process image {image_path} finsihed. Predict：{predict[0]}, Actual：{person_label}, Score：{score}")
                if (predict[0] == person_label):
                    matched_images += 1
                    break

    logger.info(f'Correct Rate：{round(matched_images / total_images * 100, 4)}%')

  
def main():
    if not os.path.exists(FACES_FEATURES_MODEL_FILE):
        clf = face_recognize_train(FACES_DIR)
        joblib.dump(clf, FACES_FEATURES_MODEL_FILE)
    
    clf = joblib.load(FACES_FEATURES_MODEL_FILE)
    face_recoginize_test(clf, FACES_DIR)
  
if __name__=="__main__":
    main()