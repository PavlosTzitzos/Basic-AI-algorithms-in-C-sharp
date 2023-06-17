using System;

class NeuralNetwork
{
    private double[,] weights;
    private double[] biases;
    private double learningRate;

    public NeuralNetwork(int inputSize, int outputSize, double learningRate)
    {
        // this is the initialization of the NN
        weights = new double[inputSize, outputSize];
        biases = new double[outputSize];
        this.learningRate = learningRate;
        InitializeWeights();
        InitializeBiases();
    }

    private void InitializeWeights()
    {
        // this function initializes the weigths of the neurons
        Random random = new Random();

        for (int i = 0; i < weights.GetLength(0); i++)
        {
            for (int j = 0; j < weights.GetLength(1); j++)
            {
                weights[i, j] = random.NextDouble() - 0.5;
            }
        }
    }

    private void InitializeBiases()
    {
        // this function adds random values in the weights of the NN
        Random random = new Random();

        for (int i = 0; i < biases.Length; i++)
        {
            biases[i] = random.NextDouble() - 0.5;
        }
    }

    private double[] Multiply(double[] input, double[,] matrix)
    {
        // this function multiplies the input vector with the input matrix
        // it is basicly a multilpier of the weights and the inputs
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        double[] result = new double[cols];

        for (int j = 0; j < cols; j++)
        {
            double sum = 0;

            for (int i = 0; i < rows; i++)
            {
                sum += input[i] * matrix[i, j];
            }

            result[j] = sum;
        }

        return result;
    }

    private double[] Add(double[] vector1, double[] vector2)
    {
        // this function adds two vectors
        int length = vector1.Length;
        double[] result = new double[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = vector1[i] + vector2[i];
        }

        return result;
    }

    private double[] ApplyActivation(double[] vector)
    {
        // this function activates the neuron
        // applies the sigmoid signal 
        int length = vector.Length;
        double[] result = new double[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = Sigmoid(vector[i]);
        }

        return result;
    }

    private double Sigmoid(double x)
    {
        // this is the activation function 
        // here we use the segmoid
        return 1 / (1 + Math.Exp(-x));
    }

    private double SigmoidDerivative(double x)
    {
        // this is the derivative of the sigmoid function
        // it is used for the gradient
        double sigmoid = Sigmoid(x);
        return sigmoid * (1 - sigmoid);
    }

    private double[] CalculateError(double[] target, double[] output)
    {
        // this function calulates the error based on the equation :
        // target - output 
        int length = target.Length;
        double[] error = new double[length];

        for (int i = 0; i < length; i++)
        {
            error[i] = target[i] - output[i];
        }

        return error;
    }

    private void UpdateWeightsAndBiases(double[] input, double[] outputError, double[] output)
    {
        // this function updates the weights after the error calculation
        int inputSize = input.Length;
        int outputSize = output.Length;

        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < outputSize; j++)
            {
                weights[i, j] += learningRate * outputError[j] * SigmoidDerivative(output[j]) * input[i];
            }
        }

        for (int j = 0; j < outputSize; j++)
        {
            biases[j] += learningRate * outputError[j] * SigmoidDerivative(output[j]);
        }
    }

    public double[] FeedForward(double[] input)
    {
        // Feed Forward function due to Feedforward NN 
        // This is the structure of the neuron :
        /*
         * input ----|
         *           x --> output --|
         * weights --|              |
         *                          + --> new output-------> final output
         *                          |                   |
         * biases ------------------|                   |
         *                                              |
         * activation ----------------------------------|
         */
        double[] output = Multiply(input, weights);
        output = Add(output, biases);
        output = ApplyActivation(output);
        return output;
    }

    public void Train(double[] input, double[] target)
    {
        // train function:
        //      produces the output
        //      calulates error
        //      updates weights
        double[] output = FeedForward(input);
        double[] outputError = CalculateError(target, output);
        UpdateWeightsAndBiases(input, outputError, output);
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Define the input and output sizes
        int inputSize = 2;
        int outputSize = 1;

        // Create a neural network with the specified input and output sizes and learning rate
        NeuralNetwork neuralNetwork = new NeuralNetwork(inputSize, outputSize, learningRate: 0.1);

        double[][] trainingData = new double[][]
        {
            new double[] { 0, 0 }, // Input
            new double[] { 0 },    // Target
            new double[] { 0, 1 },
            new double[] { 1 },
            new double[] { 1, 0 },
            new double[] { 1 },
            new double[] { 1, 1 },
            new double[] { 0 }
        };

        /*
        int N = 3; // number of bits

        int sequences = (int)Math.Pow(2,N); // calculate 2^N

        // Define the training data with 2^N lines 
        double[][] trainingData = new double[sequences][];

        for(int i = 0; i < sequences; i++)
        {
            // Convert decimal to binary string :
            string binary = Convert.ToString(i, 2).PadLeft(N, '0');
            double[] input = new double[N];
            for (int j = 0; j < N; j++)
            {
                input[j] = double.Parse(binary[j].ToString());
            }
            double[] target = new double[] { input.Sum() % 2 }; // Parity bit: 0 if even, 1 if odd

            trainingData[i] = new double[N + 1];
            Array.Copy(input, trainingData[i], N);
            trainingData[i][N] = target[0];
        }
        
        //print the target for testing :
        for (int i = 0; i < sequences; i++)
        {
            for(int j = 0; j < N; j++)
            {
                Console.Write($"{ trainingData[i][j]}");
                Console.Write(' ');
            }
            Console.WriteLine('\n');
        }*/

        // Train the neural network
        int numEpochs = 1000;

        for (int epoch = 1; epoch <= numEpochs; epoch++)
        {
            double totalLoss = 0;

            for (int i = 0; i < trainingData.Length; i += 2)
            {
                double[] input = trainingData[i];
                double[] target = trainingData[i + 1];

                neuralNetwork.Train(input, target);

                double[] output = neuralNetwork.FeedForward(input);
                double loss = CalculateLoss(output, target);
                totalLoss += loss;
            }

            if (epoch % 100 == 0)
            {
                Console.WriteLine($"Epoch: {epoch}, Loss: {totalLoss}");
            }
        }

        // Test the trained neural network
        Console.WriteLine("Testing the trained network:");
        Test(neuralNetwork, new double[] { 0, 0 });
        Test(neuralNetwork, new double[] { 0, 1 });
        Test(neuralNetwork, new double[] { 1, 0 });
        Test(neuralNetwork, new double[] { 1, 1 });

        Console.ReadLine();
    }

    static double CalculateLoss(double[] output, double[] target)
    {
        double error = 0;

        for (int i = 0; i < output.Length; i++)
        {
            error += Math.Pow(output[i] - target[i], 2);
        }

        return error / output.Length;
    }

    static void Test(NeuralNetwork neuralNetwork, double[] input)
    {
        double[] output = neuralNetwork.FeedForward(input);
        Console.WriteLine($"Input: [{string.Join(", ", input)}], Output: [{string.Join(", ", output)}]");
    }
}
