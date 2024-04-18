// using System;
// using System.Collections.Generic;
// using System.IO;
// using Emgu.CV;
// using Emgu.CV.CvEnum;
// using Emgu.CV.Structure;
// using Tensorflow;
// using static Tensorflow.Binding;

// public class ImagePredictor
// {
//     private SavedModel model;

//     public ImagePredictor(string modelPath)
//     {
//         // Load Keras model
//         model = tf.keras.models.load_model(modelPath);
//         model.
//     }

//     public List<double[]> PredictImages(List<Image<Gray, byte>> images)
//     {
//         List<double[]> predictions = new List<double[]>();
//         foreach (var img in images)
//         {
//             // Preprocess image according to your model's requirements
//             // (e.g., resize, normalize, convert to array)

//             // Example of converting Emgu.CV image to array
//             var imageData = img.ToImage<Bgr, byte>().Data;

//             // Make prediction using the loaded model
//             var prediction = model.Predict(imageData);

//             predictions.Add(prediction);
//         }
//         return predictions;
//     }
// }