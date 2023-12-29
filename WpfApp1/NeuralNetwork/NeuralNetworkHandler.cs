using System;
using static Tensorflow.Binding;
using Tensorflow;
using Tensorflow.Keras.Engine;
using System.Threading.Tasks;
using Tensorflow.NumPy;
using System.IO;
using MKT_Interface.NeuralNetwork;
namespace NeuralNetwork;

public class NeuralNetworkHandler : IDisposable
{
    Session session;
    IOptimizer optimizer;
    Model model;
    private readonly NNParameters parameters;

    public int Steps => parameters.Steps;
    public float LearningRate => (float)parameters.LearningRate;
    public int DisplayStep => parameters.DisplaySteps;

    public NeuralNetworkHandler(NNParameters parameters, TextWriter? output = null)
    {
        this.parameters = parameters;
        tf_output_redirect = output;

        session = tf.Session();
        optimizer = tf.keras.optimizers.SGD(LearningRate);
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
