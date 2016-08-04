using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace MoneySplitter
{
    [Activity(Label = "MoneySplitter", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        List<EditText> listTextMontants;
        List<EditText> listTextTotaux;
        List<Button> listBtnAdd;
        List<CheckBox> listChkTax;

        Button btnCalculate;

        List<float> montants;

        float TAX_TPS = 5;
        float TAX_TVQ = 9.975f; 

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            /*   Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };*/

            EditText textMontant1;
            EditText textMontant2;
            EditText textTotal1;
            EditText textTotal2;
            Button btnAdd1;
            Button btnAdd2;
            CheckBox chkTax1;
            CheckBox chkTax2;

            //Initialize variables
            textMontant1 = FindViewById<EditText>(Resource.Id.textMontant1);
            textMontant2 = FindViewById<EditText>(Resource.Id.textMontant2);
            textTotal1 = FindViewById<EditText>(Resource.Id.textTotal1);
            textTotal2 = FindViewById<EditText>(Resource.Id.textTotal2);

            

            //Initialize buttons
            btnAdd1 = FindViewById<Button>(Resource.Id.btnAdd1);
            btnAdd2 = FindViewById<Button>(Resource.Id.btnAdd2);

            btnCalculate = FindViewById<Button>(Resource.Id.btnCalculate);

            btnAdd1.Enabled = false;
            btnAdd2.Enabled = false;


            //Initialize CheckBox
            chkTax1 = FindViewById<CheckBox>(Resource.Id.chkTax1);
            chkTax2 = FindViewById<CheckBox>(Resource.Id.chkTax2);

            //Initialize all lists
            listBtnAdd = new List<Button>();
            listBtnAdd.Add(btnAdd1);
            listBtnAdd.Add(btnAdd2);

            listTextMontants = new List<EditText>();
            listTextMontants.Add(textMontant1);
            listTextMontants.Add(textMontant2);

            listTextTotaux = new List<EditText>();
            listTextTotaux.Add(textTotal1);
            listTextTotaux.Add(textTotal2);

            montants = new List<float>();
            montants.Add(0);
            montants.Add(0);

            listChkTax = new List<CheckBox>();
            listChkTax.Add(chkTax1);
            listChkTax.Add(chkTax2);

            foreach (EditText text in listTextMontants)
            {
                text.TextChanged += TextMontant_TextChanged;
            }

            foreach (Button btnAdd in listBtnAdd)
            {
                btnAdd.Click += BtnAdd_Click;
            }

        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            Button btnAdd = (Button)sender;
            int index = listBtnAdd.IndexOf(btnAdd);

            float montant = montants[index];
            
            if (!float.IsNaN(montant))
            {
                if (listChkTax[index].Checked)
                {
                    montant = (montant * (1 + TAX_TPS / 100.0f)) * (1 + TAX_TVQ / 100.0f);
                }
                string total = listTextTotaux[index].Text;
                if (!String.IsNullOrWhiteSpace(listTextTotaux[index].Text))
                {
                    listTextTotaux[index].Text = (float.Parse(total) + montant).ToString();
                }
                else
                {
                    listTextTotaux[index].Text = montant.ToString();
                }
            }

            //Clear TextMontants
            listTextMontants[index].Text = "";
            listTextMontants[index].Enabled = false;
        }

        private void TextMontant_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            EditText textMontant = (EditText)sender;
            if (!String.IsNullOrWhiteSpace(textMontant.Text))
            {
                int index = listTextMontants.IndexOf(textMontant);
                montants[index] = float.Parse(textMontant.Text);
                listBtnAdd[index].Enabled = true;       
            }
        }
    }
}

