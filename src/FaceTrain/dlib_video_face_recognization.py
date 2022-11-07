import cv2 
import dlib
import logging
import dlib_face_recognization as fr
import datetime
import random
import asyncio
import joblib
import fr_face_recognition as fr2

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
OUTPUT_DIR = './output/'

# 加载人脸数据库
database = list(fr.load_face_database(FACES_FEATURES_CSV_FILE))
clf = joblib.load('./data/train_data.m')

# 处理画面，特征对比方案
async def process(database, frame):
    gray = cv2.cvtColor(frame, cv2.COLOR_RGB2BGR)
    faces = detector(gray, 1)

    if len(faces) != 0:
        for i in range(len(faces)):
            face = faces[i]
            shape = predictor(gray, face)
            face_descriptor = face_reco_model.compute_face_descriptor(gray, shape)
            x, y, w, h = face.left(), face.top(), face.right() - face.left(), face.bottom() - face.top()
            cv2.rectangle(frame, (x, y), (x+w, y+h), (0, 255, 0), 1)
            face_feature_distance_list = []
            for face_data in database:
                # 比对人脸特征，当距离小于 0.4 时认为匹配成功
                dist = fr.get_euclidean_distance(face_descriptor, face_data[1])
                dist = round(dist, 4)

                if dist >= FACES_FATURES_DISTANCE_THRESHOLD:
                    continue

                face_feature_distance_list.append((face_data[0], dist))
            
            # 按距离排序，取最小值进行绘制
            sorted(face_feature_distance_list, key=lambda x:x[1])
            if face_feature_distance_list:
                person_dist = face_feature_distance_list[0][1]
                person_label = face_feature_distance_list[0][0]
                frame = fr.cv2AddChineseText(frame , f'{str(person_label)},{str(round(person_dist, 4))}', (x + 5, y - 35),(255, 0, 0), 30)
            else:
                frame = fr.cv2AddChineseText(frame , f'Unknow', (x + 5, y - 35),(255, 0, 0), 30)  

            timestamp = datetime.datetime.now().strftime('%Y-%m-%d-%H-%M-%S')
            rand = random.randint(1111, 9999)
            cv2.imwrite(f'./output/Capture-{timestamp}-{rand}.jpg', frame) 

    return frame

# 处理画面，支持向量机方案
async def process_via_svm(clf, frame):
    results = fr2.compare_face(clf, frame)
    for result in results:
        x, y, w, h = result[0]
        person_label = result[1]
        cv2.rectangle(frame, (x, y), (x+w, y+h), (0, 255, 0), 1)
        frame = fr.cv2AddChineseText(frame , f'{person_label}', (x + 5, y - 35),(255, 0, 0), 30) 
    
    if len(results) > 0:
        timestamp = datetime.datetime.now().strftime('%Y-%m-%d-%H-%M-%S')
        rand = random.randint(1111, 9999)
        cv2.imwrite(f'./output/Capture-{timestamp}-{rand}.jpg', frame) 
    return frame

async def main():
    video = cv2.VideoCapture('286370351.mp4')
    fps = video.get(cv2.CAP_PROP_FPS)

    while video.isOpened():
        ret, frame = video.read()
        if frame is None: 
            break
        
        # frame = await process(database, frame)
        frame = await process_via_svm(clf, frame)
        cv2.imshow("Face Recognization", frame)
        
        if cv2.waitKey(int(1000 / fps)) & 0xFF == ord('q'):
            break 

    video.release()
    cv2.destroyAllWindows()

if __name__ == "__main__":
    asyncio.run(main())