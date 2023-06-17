
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.IO;


public class EmergencyBrakeSystem
{
    /*
    public static void Main(string[] args)
    {
        // User inputs: car speed and distance to the object
        Console.Write("Enter the current car speed (in km/h): ");
        double speed = double.Parse(Console.ReadLine());

        Console.Write("Enter the current distance to the object (in meters): ");
        double distance = double.Parse(Console.ReadLine());

        // Fuzzy logic-based emergency brake activation
        double EmergencyBreakValue = FuzzyLogicController(speed, distance);

        // print result :
        Console.WriteLine("Emergency brake Value is: ");
        Console.WriteLine(EmergencyBreakValue);
    }*/

    public static void Main(string[] args)
    {
        int tests = 100; // number of tests 
        StreamWriter sw = new StreamWriter("C:\\Users\\pavlos\\source\\repos\\FuzzyLogic_Emergency_Break_System\\results.txt");
        sw.WriteLine("\nspeed\tdistance\tbreak\n");

        // Random inputs to give more results :
        for (int i = 0; i < tests; i++)
        {
            Random random = new Random();
            double speed = random.NextDouble() * 30;

            double distance = random.NextDouble() * 10;
            
            double EmergencyBreakValue = FuzzyLogicController(speed, distance);

            // Save results in a .csv file :

            // To write a line in buffer
            if( Double.IsNaN(EmergencyBreakValue))
            {
                EmergencyBreakValue = 0;
            }
            sw.WriteLine($"{speed}\t{distance}\t{EmergencyBreakValue}");

            // To write in output stream
            sw.Flush();
            
        }
        // To close the stream
        sw.Close();
    }

    private static double FuzzyLogicController(double speed, double distance)
    {
        //
        // Step 1: Current values into input logic (fuzzier)
        //
        double[] SpeedMFvalues = MFLogic1(speed);        // Input 1
        double[] DistanceMFvalues = MFLogic2(distance);  // Input 2
        //
        // Step 2: Rules Check (rule base)
        //
        double[] BreakValues = RuleEvaluation(SpeedMFvalues,DistanceMFvalues);
        //
        // Step 3: Calculate output (de-fuzzier)
        //
        double[] samples = sampler(BreakValues);
        double BreakValue = defuzzier(samples);
        return BreakValue;
    }

    private static double[] MFLogic1(double value)
    {
        // Here are all the settings of the membership functions :
        double[] MFvalues = {0,0,0};
        // declare the low_speed MF :
        MFvalues[0] = MembershipFunction(value,-10,0,9,12);
        // declare the medium_speed MF :
        MFvalues[1] = MembershipFunction(value,11, 14,16,19);
        // declare the high_speed MF :
        MFvalues[2] = MembershipFunction(value,18,21,30,36);
        return MFvalues;
    }

    private static double[] MFLogic2(double value)
    {
        // Here are all the settings of the membership functions :
        double[] MFvalues = { 0, 0, 0 };
        // declare the low_speed MF :
        MFvalues[0] = MembershipFunction(value, -4, 0, 3, 4);
        // declare the medium_speed MF :
        MFvalues[1] = MembershipFunction(value, 3, 4, 6, 7);
        // declare the high_speed MF :
        MFvalues[2] = MembershipFunction(value, 6, 7, 10, 11);
        return MFvalues;
    }

    private static double[] RuleEvaluation(double[] input1, double[] input2)
    {
        // Step 1: Check if rules are true
        bool rule0 = input1[2] > 0 && input2[0] > 0;
        bool rule1 = input1[0] > 0 && input2[0] > 0;
        bool rule2 = input1[0] > 0 && input2[2] > 0;
        bool rule3 = input1[2] > 0 && input2[2] > 0;

        // Step 2: Evaluate the output value
        double[] Values = { 0, 0, 0, 0 };
        if(rule0)
        {
            Values[0] = min(input1[2], input2[0]);
        }
        if(rule1)
        {
            Values[1] = min(input1[0], input2[0]);
        }
        if(rule2)
        {
            Values[2] = min(input1[0], input2[2]);
        }
        if(rule3)
        {
            Values[3] = min(input1[2], input2[2]);
        }

        return Values;
    }
    private static double[] MFLogic3(double value)
    {
        // Here are all the settings of the membership functions :
        double[] MFvalues = { 0, 0, 0 };
        // declare the low_speed MF :
        MFvalues[0] = MembershipFunction(value, -0.1, 0, 0.1, 0.2);
        // declare the medium_speed MF :
        MFvalues[1] = MembershipFunction(value, 0.1, 0.2, 0.5, 0.6);
        // declare the high_speed MF :
        MFvalues[2] = MembershipFunction(value, 0.5, 0.6, 1, 1.1);
        return MFvalues;
    }
    private static double MembershipFunction(double x, double a, double b, double c, double d)
    {
        /* trapezodial waveform :
         *           b        c
         * 1 |       o--------o
         *   |      /          \
         *   |     /            \
         * 0 |----o              o---
         * ^      a              d
         * |
         * ---- output value range : 0 ~ 1
         */

        if (x <= a || x >= d)
        {
            return 0;
        }
        else if (x >= b && x <= c)
        {
            return 1;
        }
        else if (x > a && x < b)
        {
            return (x - a) / (b - a);
        }
        else
        {
            return (d - x) / (d - c);
        }
    }

    private static double min(double a, double b)
    {
        // minimum of two double numbers
        return Math.Min(a, b);
    }

    private static double max(double a, double b)
    {
        //maximum of two double numbers
        return Math.Max(a, b);
    }

    private static double[] sampler(double[] rules)
    {
        double min = 0;
        double max = 1;
        int N = 100;
        double step = (max - min) / N;
        double[] index = new double[N];
        for(int i = 0; i<N;i++)
        {
            index[i] = step*i;
        }
        double[] sample = new double[N];
        for (int i = 0; i<N;i++)
        {
            sample[i] = rules[0] * MFLogic3(index[i])[2] + rules[1] * MFLogic3(index[i])[1] + rules[2] * MFLogic3(index[i])[0] + rules[3] * MFLogic3(index[i])[1];
        }
        return sample;
    }
    private static double defuzzier(double[] samples)
    {
        // This is the average defuzzier

        //double sum = 0;
        //for (int i = 0;i< rule_outputs.Length; i++)
        //{
        //    sum += rule_outputs[i];
        //}
        //double avg = sum / rule_outputs.Length;
        //return avg;

        // This is the discrete centroid defuzzier
        int N = samples.Length;
        double min = 0;
        double max = 1;
        double step = (max - min) / N;
        double[] index = new double[N];
        for (int i = 0; i < N; i++)
        {
            index[i] = step * i;
        }
        double sum_1 = 0;
        double sum_2 = 0;
        for (int i = 0; i< N; i++)
        {
            sum_1 += index[i] * samples[i];
            sum_2 += samples[i];
        }
        double y_output = sum_1 / sum_2;
        return y_output;
    }
}
