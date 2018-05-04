using System;
using System.Collections.Generic;

namespace BGBA.Services.N.Autenticacion.SCS
{
    internal class Scrambler
    {
        static int userLength = 35;

        static int passwordLength = 8;

        static int chainLenght = 50;

        static int blockSize = 5;

        static List<String> firstList = new List<String>(new string[] { "A", "E", "I", "M", "Q", "U", "Y" });

        static List<String> secondList = new List<String>(new string[] { "B", "F", "J", "N", "R", "V", "Z" });

        static List<String> thirdList = new List<String>(new string[] { "C", "G", "K", "O", "S", "W" });

        static List<String> fourthList = new List<string>(new string[] { "D", "H", "L", "P", "T", "X" });

        //  static final int offSetBlock8 = 6;
        public String scrambler(User user)
        {
            int initialPos = 0;
            String tmpScr = "";
            String tmp = this.padd(user);
            for (int i = 0; i < chainLenght / blockSize; i++)
            {
                initialPos = blockSize * i;
                tmpScr += this.substitution(Utils.jsubstring(tmp, initialPos, initialPos + blockSize));
            }

            String toReturn = tmpScr;
            //  + Utils.generateRandomString(offSetBlock8);
            return toReturn;
        }

        private String padd(User user)
        {
            //  Se queda con el valor mas corto
            int muserIdLenght = (user.UserId.Length > userLength) ? userLength : user.UserId.Length;
            int muserPassLenght = (user.Password.Length > passwordLength) ? passwordLength : user.Password.Length;

            String tempUserId = (user.UserId + Utils.generateRandomString(true, (userLength - muserIdLenght)));
            String tempUserPass = (user.Password + Utils.generateRandomString(true, (passwordLength - muserPassLenght)));

            String toReturn = tempUserId
                        + tempUserPass
                        + String.Format("{0:00}", muserIdLenght)
                        + String.Format("{0:00}", muserPassLenght)
                        + Utils.generateRandomString(true, chainLenght - userLength - passwordLength - 4);
            return toReturn;
        }

        private String substitution(String data)
        {

            String algSubs = Utils.generateRandomString(false, 1);
            String dataScram = "";
            int[] substitutionPositions = null;

            if (firstList.Contains(algSubs))
            {
                substitutionPositions = new int[] { 7, 3, 0, 1, 4 };
            }
            else
            {
                if (secondList.Contains(algSubs))
                {
                    substitutionPositions = new int[] { 3, 7, 6, 5, 1 };
                }
                else
                {
                    if (thirdList.Contains(algSubs))
                    {
                        substitutionPositions = new int[] { 4, 0, 3, 6, 5 };
                    }
                    else
                    {
                        if (fourthList.Contains(algSubs))
                        {
                            substitutionPositions = new int[] { 5, 1, 4, 0, 6 };
                        }
                    }
                }
            }

            if (substitutionPositions != null)
            {
                dataScram = getDataScram(substitutionPositions, data);
            }
            else
            {
                dataScram = Utils.generateRandomString(true, 8);
            }

            dataScram = algSubs + dataScram;
            return dataScram;
        }

        private String getDataScram(int[] substitutionPositions, String sourceData)
        {

            String resultString = Utils.generateRandomString(true, 8);
            String firstPart = "";
            String substitutionString = "";
            String secondPart = "";

            for (int i = 0; i < substitutionPositions.Length; i++)
            {
                firstPart = Utils.jsubstring(resultString, 0, substitutionPositions[i]);
                substitutionString = Utils.jsubstring(sourceData, i, i + 1);
                secondPart = Utils.jsubstring(resultString, substitutionPositions[i] + 1, resultString.Length);

                resultString = firstPart + substitutionString + secondPart;
            }

            return resultString;
        }
    }
}
