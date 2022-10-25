import os
import dlib
from PIL import Image, ImageDraw, ImageFont
import cv2
import numpy as np
import csv
import logging
import uuid

# Logging
logger = logging.getLogger(__name__)
logging.basicConfig(level=logging.INFO)

# Dlib 正向人脸检测器
detector = dlib.get_frontal_face_detector()

# Dlib 人脸特征点检测器
predictor = dlib.shape_predictor('data/data_dlib/shape_predictor_68_face_landmarks.dat')

# Dlib 人脸识别模型
face_reco_model = dlib.face_recognition_model_v1("data/data_dlib/dlib_face_recognition_resnet_model_v1.dat")

# 常量定义
FACES_DIR = './faces/'
FACES_FEATURES_CSV_FILE = './data/features_all.csv'
FACES_FATURES_DISTANCE_THRESHOLD = 0.4
OUTPUT_DIR = './ouput/'

def get_mean_features_of_face(path):
    path = os.path.abspath(path)
    subDirs = [os.path.join(path, f) for f in os.listdir(path)]
    subDirs= list(filter(lambda x:os.path.isdir(x), subDirs))
    for index in range(0, len(subDirs)):
        subDir = subDirs[index]
        person_label = os.path.split(subDir)[-1]
        image_paths = [os.path.join(subDir, f) for f in os.listdir(subDir)]
        image_paths = list(filter(lambda x:os.path.isfile(x), image_paths))
        feature_list_of_person_x = []
        for image_path in image_paths:
            if os.path.split(image_path)[-1].split(".")[-1] != "jpg":
                continue
            
            # 计算每一个图片的特征
            feature = get_128d_features_of_face(image_path)
            if feature == 0:
                logger.warning(f"The image '{image_path}' can not extract face feature.")
                continue
            
            feature_list_of_person_x.append(feature)
            logger.info(f"Extracting face feature from image '{image_path}' finished.")
        
        # 计算当前人脸的平均特征
        features_mean_person_x = np.zeros(128, dtype=object, order='C')
        if feature_list_of_person_x:
            features_mean_person_x = np.array(feature_list_of_person_x, dtype=object).mean(axis=0)
        
        logger.info(f"Calculating face feature for person '{image_path}' finished.")
        yield (features_mean_person_x, person_label)

def get_128d_features_of_face(image_path):
    image = Image.open(image_path)
    image = cv2.cvtColor(np.asarray(image), cv2.COLOR_RGB2BGR)
    faces = detector(image, 1)

    if len(faces) != 0:
        shape = predictor(image, faces[0])
        face_descriptor = face_reco_model.compute_face_descriptor(image, shape)
    else:
        face_descriptor = 0
    return face_descriptor

def extract_features_to_csv(faces_dir):
    mean_features_list = list(get_mean_features_of_face(faces_dir))
    with open("data/features_all.csv", "w", newline="") as csvfile:
        writer = csv.writer(csvfile)
        for mean_features in mean_features_list:
            person_features = mean_features[0]
            person_label = mean_features[1]
            person_features = np.insert(person_features, 0, person_label, axis=0)
            writer.writerow(person_features)
            logger.info(f"Writing face feature for person '{person_label}' finished.")

def get_euclidean_distance(feature_1, feature_2):
    feature_1 = np.array(feature_1)
    feature_2 = np.array(feature_2)
    return np.sqrt(np.sum(np.square(feature_1 - feature_2)))

def load_face_database(csv_file):
    if os.path.exists(csv_file):
        with open(csv_file) as fr:
            rows = csv.reader(fr)        
            for row in rows:
                float_array = []
                for i in range(1, 129):
                    float_array.append(float(row[i]))
                yield row[0], np.array(float_array, dtype=object)

def compare_face_fatures_with_database(database, image_path):
    image = Image.open(image_path)
    image = cv2.cvtColor(np.asarray(image), cv2.COLOR_RGB2BGR)
    faces = detector(image, 1)
    
    campare_results = []
    if len(faces) != 0:
        for i in range(len(faces)):
            face = faces[i]
            shape = predictor(image, faces[0])
            face_descriptor = face_reco_model.compute_face_descriptor(image, shape)
            x, y, w, h = face.left(), face.top(), face.right() - face.left(), face.bottom() - face.top()
            cv2.rectangle(image, (x, y), (x+w, y+h), (0, 255, 0), 1)
            face_feature_distance_list = []
            for face_data in database:
                # 比对人脸特征，当距离小于 0.4 时认为匹配成功
                dist = get_euclidean_distance(face_descriptor, face_data[1])
                dist = round(dist, 4)

                if dist >= FACES_FATURES_DISTANCE_THRESHOLD:
                    continue

                face_feature_distance_list.append((face_data[0], dist))
            
            # 按距离排序，取最小值进行绘制
            sorted(face_feature_distance_list, key=lambda x:x[1])
            if face_feature_distance_list:
                person_dist = face_feature_distance_list[0][1]
                person_label = face_feature_distance_list[0][0]
                image = cv2AddChineseText(image , f'{str(person_label)},{str(round(person_dist, 4))}', (x + 5, y - 35),(255, 0, 0), 30)
                campare_results.append((person_label, person_dist))
        
        # 输出人脸比对结果
        output_image = os.path.split(image_path)[-1].split('.')[0] + '_Output.jpg'
        output_image = os.path.join(OUTPUT_DIR, output_image)
        cv2.imwrite(output_image, image)

    return campare_results

def cv2AddChineseText(img, text, position, textColor=(0, 255, 0), textSize=30):
    if (isinstance(img, np.ndarray)):
        img = Image.fromarray(cv2.cvtColor(img, cv2.COLOR_BGR2RGB))

    draw = ImageDraw.Draw(img)
    fontStyle = ImageFont.truetype("simsun.ttc", textSize, encoding="utf-8")
    draw.text(position, text, textColor, font=fontStyle)
    return cv2.cvtColor(np.asarray(img), cv2.COLOR_RGB2BGR)

def main():
    if not os.path.exists(OUTPUT_DIR):
        os.makedirs(OUTPUT_DIR)

    total_images = 0
    matched_images = 0
    
    # 加载人脸特征数据库
    database = list(load_face_database(FACES_FEATURES_CSV_FILE))

    # 加载测试人脸数据
    faces_dir = os.path.abspath(FACES_DIR)
    subDirs = [os.path.join(faces_dir, f) for f in os.listdir(faces_dir)]
    subDirs = list(filter(lambda x:os.path.isdir(x), subDirs))
    for subDir in subDirs:
        image_paths = [os.path.join(subDir, f) for f in os.listdir(subDir)]
        image_paths = list(filter(lambda x:os.path.isfile(x), image_paths))
        total_images += len(image_paths)
        for image_path in image_paths:
            result = compare_face_fatures_with_database(database, image_path)
            if len(result) == 0:
                logger.warning(f"The image '{image_path}' can not be detected.")
                continue

            sorted(result, key=lambda x:x[1])
            predict = result[0][0]
            actual = os.path.split(subDir)[-1]
            logger.info(f"Process image {image_path} finsihed. Predict：{predict}, Actual：{actual}，Distance：{result[0][1]}")
            if predict == actual:
                matched_images += 1

    logger.info(f'Correct Rate：{round(matched_images / total_images * 100, 4)}%')

if __name__ == '__main__':
    main()