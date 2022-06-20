using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace LynxCmd
{/// <summary>
/// Class for encodeing / decoding EtherLynx messages
/// </summary>
   class LynxMsg
    {
        
        public byte[] LynxEncode(string cmd) {
            byte[] b;
            string s = "303031313030503132303800496E7665727465722047726F7570000000000000000000000D2818020000000C0000000000000000010000000A88";

            //if (cmd == 'ProductionDayMinus') {
            //    //TODO 
            //}


            switch (cmd) {
                case "EnergyNow":
                    s = s + "010100000000";
                    break;
                case "TotalProduction":
                    s = s + "010200000000";
                    break;
                case "EnergyL1":
                    s = s + "024200000000";
                    break;
                case "EnergyL2":
                    s = s + "024300000000";
                    break;
                case "EnergyL3":
                    s = s + "024400000000";
                    break;
                case "CurrentL1":
                    s = s + "023F00000000";
                    break;
                case "CurrentL2":
                    s = s + "024000000000";
                    break;
                case "CurrentL3":
                    s = s + "024100000000";
                    break;
                case "ProductionToday":
                    s = s + "780A00000000";
                    break;
                case "ProductionDayMinus":

                    break;
                case "ProductionThisWeek":
                    s = s + "781400000000";
                    break;
                case "ProductionLastWeek":
                    s = s + "781500000000";
                    break;
                case "ProductionThisMonth":
                    s = s + "781E00000000";
                    break;
                case "ProductionLastMonth":  //Last month does not excist, 31-42 = jan to dec..
                    int m = DateTime.Now.Month; //1-12
                    m = m -1;
                    if (m == 0)
                        m = 12;   //If January then last month = 12
                    m = m + 30;
                    s =  s + "78" + m.ToString("X") + "00000000";
                break;
                case "ProductionThisYear":
                    s = s + "783200000000";
                    break;
                case "ProductionLastYear":
                    s = s + "783300000000";
                    break;
                case "Opmode":
                    s = s + "0A0200000000";
                    break;
                case "GridFreq":
                    s = s + "025000000000";
                    break;
                case "GridVoltL1":  //L1 mean 10 min
                    s = s + "025B00000000"; 
                    break;
                case "GridVoltL2":  //L1 mean 10 min
                    s = s + "025C00000000";
                    break;
                case "GridVoltL3":  //L1 mean 10 min
                    s = s + "025D00000000";
                    break;

                default:
                    s = null;
                break;
            }

            if (s != null)
            {
                 b = StringToByteArray(s);
            }
            else {
                 b = null;
            }
           
            return b;
        }

        public string LynxDecode(byte[] b) {
            string s = "";
            try
            {
                Array.Reverse(b, b.Length - 4, 4);  //Reverse the 4 bytes containing the result
                int value = 0;
                byte[] res = new byte[4];
                Array.Copy(b, 60, res, 0, 4);
                value = BitConverter.ToInt32(res, 0);
                double ff = value;
                NumberFormatInfo nfi = new CultureInfo("da-DK", false).NumberFormat;
                switch (b[58]) {   //TODO dis is F**** stupid, ther must be a better way
                    case 01:
                        switch (b[59]) {
                            case 01:
                                s = "Energy now is " + value + " W";
                                break;
                            case 02:
                                if (value.ToString().Length > 5)
                                { //Convert to mVh
                                    double f = value;
                                    f = f / 1000000;
                                    s = "Inverter total production is " + f.ToString("N", nfi) + " MWh";

                                }
                                else {
                                    double f = value;
                                    f = f / 1000;
                                    s = "Inverter total production is " + f.ToString("N", nfi) + " kWh";
                                }
                                break;
                            case 04:
                               
                                break;

                        }

                        break;
                    case 02:
                        
                        switch (b[59])
                        {
                           
                            case 63:  //3F
                                ff = ff / 1000;
                                s = "L1 current is " + ff.ToString("N", nfi) + " A";
                                break;
                            case 64:  //40
                                ff = ff / 1000;
                                s = "L2 current is " + ff.ToString("N", nfi) + " A";
                                break;
                            case 65:  //41
                                ff = ff / 1000;
                                s = "L3 current is " + ff.ToString("N", nfi) + " A";
                                break;
                            case 66:  //42
                                s = "L1 power is " + ff.ToString("N", nfi) + " W";
                                break;
                            case 67:  //43
                                s = "L2 power is " + ff.ToString("N", nfi) + " W";
                                break;
                            case 68:  //44
                                s = "L3 power is " + ff.ToString("N", nfi) + " W";
                                break;
                            case 80:  //Todo maybe apply , ?
                                s = "Frequency = " + (ff / 1000).ToString() + " hz";
                                break;
                            case 91:  //L1 Mean 10 min
                                // ff = ff / 1000;
                                s = "L1 mean = " + (ff / 10).ToString() + " v";
                                break;
                            case 92:  //L2 Mean 10 min
                                // ff = ff / 1000;
                                s = "L2 mean = " + (ff / 10).ToString() + " v";
                                break;
                            case 93:  //L2 Mean 10 min
                                // ff = ff / 1000;
                                s = "L3 mean = " + (ff / 10).ToString() + " v";
                                break;
                        }

                        break;

                    case 10:
                        switch (b[59]) {
                            case 2:
                                int a = int.Parse(ff.ToString());
                                if (a >= 0 & a <= 9) {
                                    a = 1;
                                }

                                if (a >= 10 & a <= 49)
                                {
                                    a = 10;
                                }

                                if (a >= 50 & a <= 59)
                                {
                                    a = 50;
                                }
                                if (a >= 60 & a <= 69)
                                {
                                    a = 60;
                                }
                                if (a >= 70 & a <= 79)
                                {
                                    a = 70;
                                }
                                if (a >= 80 & a <= 89)
                                {
                                    a = 80;
                                }
                                switch (a) {
                                    case 1:
                                        s = "Opmode = " + ff.ToString() + " Shut down";
                                        break;
                                    case 10:
                                        s = "Opmode = " + ff.ToString() + " Booting";
                                        break;
                                    case 50:
                                        s = "Opmode = " + ff.ToString() + " Prepare to connect";
                                        break;
                                    case 60:
                                        s = "Opmode = " + ff.ToString() + " On grid";
                                        break;
                                    case 70:
                                        s = "Opmode = " + ff.ToString() + " Disconnected due to error";
                                        break;
                                    case 80:
                                        s = "Opmode = " + ff.ToString() + " Shut down";
                                        break;
                                    default:
                                        s = "Unknown opmode / fail";
                                        break;
                                }
                                
                                break;
                                
                        }

                        break;
                    case 120:  //0x78

                        if (b[59] >= 31 & b[59] <= 42)
                            b[59] = 31;         //Force last month to 31


                        switch (b[59])
                        {
                            case 10:  //0x0A
                                ff = ff / 1000;
                                s = "Production today is " + ff.ToString("N", nfi) + " kWh";
                                break;
                            case 11: //0x0B
                                ff = ff / 1000;
                                s = "Production last monday was " + ff.ToString("N", nfi) + " kWh";
                                break;
                            case 12: //0x0C
                                ff = ff / 1000;
                                s = "Production last tuesday was " + ff.ToString("N", nfi) + " kWh";
                                break;
                            case 13: //0x0D
                                ff = ff / 1000;
                                s = "Production last wednesday was " + ff.ToString("N", nfi) + " kWh";
                                break;
                            case 14: //0x0E
                                ff = ff / 1000;
                                s = "Production last thuesday was " + ff.ToString("N", nfi) + " kWh";
                                break;
                            case 15: //0x0F
                                ff = ff / 1000;
                                s = "Production last friday was " + ff.ToString("N", nfi) + " kWh";
                                break;
                            case 16: //0x10
                                ff = ff / 1000;
                                s = "Production last saturday was " + ff.ToString("N", nfi) + " kWh";
                                break;
                            case 17: //0x11
                                ff = ff / 1000;
                                s = "Production last sunday was " + ff.ToString("N", nfi) + " kWh";
                                break;
                            case 20: //0x14
                                ff = ff / 1000;
                                s = "Production this week is " + ff.ToString("N", nfi) + " kWh";
                                break;
                            case 21: //0x15
                                ff = ff / 1000;
                                s = "Production last week was " + ff.ToString("N", nfi) + " kWh";
                                break;

                            case 30: //0x1E
                                ff = ff / 1000;
                                s = "Production this month is " + ff.ToString("N", nfi) + " kWh";
                                break;
                            case 31:  //Last month is always = 31 see begining of switch case
                                ff = ff / 1000;
                                s = "Production last month was " + ff.ToString("N", nfi) + " kWh";
                                break;
                            case 50: //0x32
                                ff = ff / 1000;
                                s = "Production this year is " + ff.ToString("N", nfi) + " kWh";
                                break;
                           case 51: //0x33
                                ff = ff / 1000;
                                s = "Production last year was " + ff.ToString("N", nfi) + " kWh";
                                break;
                        }

                        break;


                }



            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            
            

            return s;
        }

        private static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                string ss = hex.Substring(i, 2);
                int a = Int32.Parse(ss, System.Globalization.NumberStyles.HexNumber);
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            
            return bytes;
        }

    }
}
