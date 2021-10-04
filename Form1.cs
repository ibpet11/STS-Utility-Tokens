using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STSTokens
{
    public partial class Form1 : Form
    {
        UtilityHelper UtiHelper = new UtilityHelper();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var tokenKlass = 0;
            var tokenSubKlass = 0;
            var randum = 7;
            double utilityAmount = Convert.ToDouble(TxtAmount.Text);
            var issueDate = new DateTime(issueDt.Value.Year, issueDt.Value.Month, issueDt.Value.Day, issueDt.Value.Hour, issueDt.Value.Minute, issueDt.Value.Second);
            var baseDate = new DateTime(1993, 1, 1, 0, 0, 0);

            var getKlassBlock = Task.Run(() => UtiHelper.dec2Bin(tokenKlass, 2)).Result;
            var getSubKlassBlock = Task.Run(() => UtiHelper.dec2Bin(randum, 4)).Result;
            var getRNDBlock = Task.Run(() => UtiHelper.dec2Bin(randum, 4)).Result;
            //TimeSpan varTime = baseDate - issueDate;
            TimeSpan varTime = issueDate - baseDate;
            double fractMinutes = varTime.TotalMinutes;
            int realMinutes = (int)fractMinutes;
            var getTIDBlock = Task.Run(() => UtiHelper.dec2Bin(realMinutes, 24)).Result;

            int compAmount = (int)(utilityAmount * 10);
            int exponent = Task.Run(() => UtiHelper.getExponent(compAmount)).Result;
            int mantissa = Task.Run(() => UtiHelper.getMantissa(exponent, compAmount)).Result;
            var val1 = Task.Run(() => UtiHelper.dec2Bin(exponent, 2)).Result;
            var val2 = Task.Run(() => UtiHelper.dec2Bin(mantissa, 14)).Result;
            var getAmountBlock = val1 + val2;

            string crc = $"{getKlassBlock}{getSubKlassBlock}{getRNDBlock}{getTIDBlock}{getAmountBlock}";
            var crcBlock = Task.Run(() => UtiHelper.getCRCBlock(crc)).Result;
            var token64BitBlk = $"{getKlassBlock}{getRNDBlock}{getTIDBlock}{getAmountBlock}{crcBlock}";

            TxtTokenBlk.Text = token64BitBlk;
        }
    }
}
