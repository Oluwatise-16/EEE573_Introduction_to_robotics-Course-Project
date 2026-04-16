using System;
using System.IO;
using System.Collections.Generic;

namespace PULSR_3
{
    public class StudentController
    {
        private const double L1 = 26.0;
        private const double L2 = 52.0;
        private const double SCALER = 25.0;
        private const double OFFSET_ANGLE = 20.0;

        private List<int> upperCommands;
        private List<int> lowerCommands;

        public StudentController()
        {
            upperCommands = new List<int>();
            lowerCommands = new List<int>();
            LoadTrajectoryFiles();
        }

        public int[] ForwardKinematics(double theta1, double theta2)
        {
            // TODO Step 1: Convert degrees to radians
            double rad1 = theta1 * (Math.PI / 180.0);
            double rad2 = theta2 * (Math.PI / 180.0);
            double radOffset = OFFSET_ANGLE * (Math.PI / 180.0);

            // TODO Step 2: Calculate link positions using trig
            double e2 = (L1 * Math.Cos(rad1)) + (L2 * Math.Cos(rad2));
            double e1 = (L1 * Math.Sin(rad1)) + (L2 * Math.Sin(rad2));

            // TODO Step 3: Apply coordinate transformation (20 degree offset)
            double x_screen = ((e1 * Math.Cos(radOffset)) - (e2 * Math.Sin(radOffset)));
            double y_screen = (-(e2 * Math.Cos(radOffset)) - (e1 * Math.Sin(radOffset)));

            // TODO Step 4: Apply scaler (multiply by SCALER)
            int finalX = (int)(x_screen * SCALER);
            int finalY = (int)(y_screen * SCALER);

            return new int[] { finalX, finalY };
        }

        private void LoadTrajectoryFiles()
        {    
            try {
                upperCommands.Clear();
                lowerCommands.Clear();

                // TODO: Read "Nupper_targets.txt" 
                string[] upperLines = File.ReadAllLines("Nupper_targets.txt");
                foreach (string line in upperLines) {
                    if (int.TryParse(line, out int val)) upperCommands.Add(val);
                }
                
                // TODO: Read "Nlower_targets.txt"
                string[] lowerLines = File.ReadAllLines("Nlower_targets.txt");
                foreach (string line in lowerLines) {
                    if (int.TryParse(line, out int val)) lowerCommands.Add(val);
                }
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public int[] CalculateControl(int robotX, int robotY, double orbitAngle)
        { 
            // TODO Step 1: Calculate which step we are on (270 to -90)
            int step = (int)(270 - orbitAngle);

            // TODO Step 2: Make sure step is in valid range
            if (step < 0) step = 0;
            if (step > 359) step = 359;

            // TODO Step 3: Get the motor speeds from the loaded lists
            int speedUpper = 0;
            int speedLower = 0;

            if (upperCommands.Count > step && lowerCommands.Count > step) {
                speedUpper = upperCommands[step];
                speedLower = lowerCommands[step];
            }

            return new int[] { speedUpper, speedLower };
        }
    }
}