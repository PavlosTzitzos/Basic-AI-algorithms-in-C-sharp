using System;
using System.Collections.Generic;
using System.Text;

namespace Simple_GeneticAlgorithm
{
    public class Simple_GeneticAlgorithm
    {
        //population size
        private int POP = 30;
        //geneotype
        private int LEN = 10;
        //mutation rate, change it have a play
        private double MUT = 0.1;
        //recomination rate
        private double REC = 0.5;
        //how many tournaments should be played
        private double END = 1000;
        //the sum pile, end result for the SUM pile
        //card1 + card2 + card3 + card4 + card5, MUST = 36 for a good GA
        private double SUMTARG = 36;
        //the product pile, end result for the PRODUCT pile
        //card1 * card2 * card3 * card4 * card5, MUST = 360 for a good GA
        private double PRODTARG = 360;
        //the genes array, 30 members, 10 cards each
        private int[,] gene = new int[30, 10];
        //used to create randomness (Simulates selection process in nature)
        //randomly selects genes
        Random rnd = new Random();

        //empty constructor
        public Simple_GeneticAlgorithm()
        {
        }

        //Runs the Microbial GA to solve the problem domain
        //Where the problem domain is specified as follows
        //
        //You have 10 cards numbered 1 to 10.
        //You have to divide them into 2 piles so that:
        //
        //The sum of the first pile is as close as possible to 36
        //And the product of all in second pile is as close as poss to 360
        public void run()
        {
            //declare pop member a,b, winner and loser
            int a, b, Winner, Loser;
            //initialise the population (randomly)
            init_pop();
            //start a tournament
            for (int tournamentNo = 0; tournamentNo < END; tournamentNo++)
            {
                //pull 2 population members at random
                a = (int)(POP * rnd.NextDouble());
                b = (int)(POP * rnd.NextDouble());
                //have a fight, see who has best genes
                if (evaluate(a) < evaluate(b))
                {
                    Winner = a;
                    Loser = b;
                }
                else
                {
                    Winner = b;
                    Loser = a;
                }
                //Possibly do some gene jiggling, on all genes of loser
                //again depends on randomness (simulating the 
                //natural selection
                //process of evolutionary selection)
                for (int i = 0; i < LEN; i++)
                {
                    //maybe do some recombination
                    if (rnd.NextDouble() < REC)
                        gene[Loser, i] = gene[Winner, i];
                    //maybe do some muttion
                    if (rnd.NextDouble() < MUT)
                        gene[Loser, i] = 1 - gene[Loser, i];
                    //then test to see if the new population member 
                    //is a winner
                    if (evaluate(Loser) == 0.0)
                        display(tournamentNo, Loser);
                }
            }
        }


        //Display the results. Only called for good GA which has solved
        //the problem domain
        //@param tournaments : the current tournament loop number
        //@param n : the nth member of the population. 
        private void display(int tournaments, int n)
        {
            Console.WriteLine("\r\n==============================\r\n");
            Console.WriteLine("After " + tournaments + 
                              " tournaments, Solution sum pile " + 
                              "(should be 36) cards are : ");
            for (int i = 0; i < LEN; i++) {
              if (gene[n,i] == 0) {
                Console.WriteLine(i + 1);
              }
            }
            Console.WriteLine("\r\nAnd Product pile " + 
                              "(should be 360)  cards are : ");
            for (int i = 0; i < LEN; i++) {
              if (gene[n,i] == 1) {
                  Console.WriteLine(i + 1);
              }
            }
        }

        //evaluate the the nth member of the population
        //@param n : the nth member of the population
        //@return : the score for this member of the population.
        //If score is 0.0, then we have a good GA which has solved
        //the problem domain
        private double evaluate(int n)
        {
            //initialise field values
            int sum = 0, prod = 1;
            double scaled_sum_error, scaled_prod_error, combined_error;
            //loop though all genes for this population member
            for (int i = 0; i < LEN; i++)
            {
                //if the gene value is 0, then put it in 
                //the sum (pile 0), and calculate sum
                if (gene[n,i] == 0)
                {
                    sum += (1 + i);
                }
                //if the gene value is 1, then put it in 
                //the product (pile 1), and calculate sum
                else
                {
                    prod *= (1 + i);
                }
            }
            //work out how food this population member is, 
            //based on an overall error
            //for the problem domain
            //NOTE : The fitness function will change 
            //       for every problem domain.
            scaled_sum_error = (sum - SUMTARG) / SUMTARG;
            scaled_prod_error = (prod - PRODTARG) / PRODTARG;
            combined_error = Math.Abs(scaled_sum_error) + 
                             Math.Abs(scaled_prod_error);

            return combined_error;
        }

        //initialise population
        private void init_pop()
        {
            //for entire population
            for (int i = 0; i < POP; i++)
            {
                //for all genes
                for (int j = 0; j < LEN; j++)
                {
                    //randomly create gene values
                    if (rnd.NextDouble() < 0.5)
                    {
                        gene[i,j] = 0;
                    }
                    else
                    {
                        gene[i,j] = 1;
                    }
                }
            }
        }
    }
    
    class Program
    {
        //main access point
        static void Main(string[] args)
        {
            //create a new Microbial GA
            Simple_GeneticAlgorithm GA = new Simple_GeneticAlgorithm();
            GA.run();
            //read a line, to stop the Console window closing
            Console.ReadLine();
        }
    }
}
