using System;
using static Tensorflow.Binding;
using Tensorflow;
using Tensorflow.Keras.Engine;
using System.Threading.Tasks;
using Tensorflow.NumPy;
using System.IO;
namespace NeuralNetwork;


public class NeuralNetworkHandler : IDisposable
{
    Session session;
    IOptimizer optimizer;

    public int Steps { get; }
    public float LearningRate { get; }
    public int DisplayStep { get; }
    Model model;

    public NeuralNetworkHandler(int steps, float learningRate, int displayStep, TextWriter? output = null)
    {
        tf_output_redirect = output;
        session = tf.Session();
        optimizer = tf.keras.optimizers.SGD(learningRate);
        Steps = steps;
        LearningRate = learningRate;
        DisplayStep = displayStep;
    }


    public void Dispose()
    {
        session.Dispose();
    }

    public async void TrainModelAsync(NDArray X, NDArray Y)
    {
        await Task.Run(() =>
        {
            

            // STRAIGHT-FORWARD
            //var W = tf.Variable(-0.06f, name: "weight");
            //var b = tf.Variable(-0.73f, name: "bias");
            //var n = X.shape[0];
            //for (int i = 1; i <= Steps; i++)
            //{
            //    using var g = tf.GradientTape();
            //    var pred = W * X + b;

            //    var loss = tf.reduce_sum(tf.pow(pred - Y, 2) / n, name: "loss");

            //    var grads = g.gradient(loss, (W, b));

            //    optimizer.apply_gradients(zip(grads, (W, b)));

            //    if (i % DisplayStep == 0){
            //        pred = W * X + b;
            //        loss = tf.reduce_sum(tf.pow(pred - Y, 2) / n, name: "loss");
                    
            //        print(loss);
            //    }
            //}
        });
    }
    public void SaveModel()
    {

    }


}
