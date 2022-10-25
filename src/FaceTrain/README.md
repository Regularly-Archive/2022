这是一个人脸识别的实验项目，含下面两个项目：

* dlib_face_recognization：基于 dlib 的人脸识别，原理是计算出某张人脸特征的平均值，然后计算和目标人脸特征的欧式距离，距离越小，表示两张人脸越接近，正确率可以达到 94 %
* fr_face_recognition：基于 face_recognition 的人脸识别，结合支持向量机来预测目标人脸，正确率可以达到 98% 