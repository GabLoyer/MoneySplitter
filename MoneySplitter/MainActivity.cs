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
        List<Button> listBtnAddMtn;
        List<CheckBox> listChkTax;
        List<EditText> listPeoples;
        List<Spinner> listSpinners;

        Button btnCalculate;

        LinearLayout layoutResult;

        List<float> montants;

        float TAX_TPS = 5;
        float TAX_TVQ = 9.975f;

        int NbPeople;

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
            EditText textName1;
            EditText textName2;
            EditText textTotal1;
            EditText textTotal2;
            Button btnAddMtn1;
            Button btnAddMtn2;
            CheckBox chkTax1;
            CheckBox chkTax2;
            Button btnAddPeople;

            Spinner spinnerWeight1;
            Spinner spinnerWeight2;

            //Initialize variables
            textMontant1 = FindViewById<EditText>(Resource.Id.textMontant1);
            textMontant2 = FindViewById<EditText>(Resource.Id.textMontant2);
            textTotal1 = FindViewById<EditText>(Resource.Id.textTotal1);
            textTotal2 = FindViewById<EditText>(Resource.Id.textTotal2);
            textName1 = FindViewById<EditText>(Resource.Id.textName1);
            textName2 = FindViewById<EditText>(Resource.Id.textName2);


            //Initialize buttons
            btnAddMtn1 = FindViewById<Button>(Resource.Id.btnAdd1);
            btnAddMtn2 = FindViewById<Button>(Resource.Id.btnAdd2);

            btnCalculate = FindViewById<Button>(Resource.Id.btnCalculate);

            btnAddPeople = FindViewById<Button>(Resource.Id.btnPeople);

            btnAddMtn1.Enabled = false;
            btnAddMtn2.Enabled = false;

            
            //Initialize CheckBox
            chkTax1 = FindViewById<CheckBox>(Resource.Id.chkTax1);
            chkTax2 = FindViewById<CheckBox>(Resource.Id.chkTax2);

            //Initialize Spinner 
            spinnerWeight1 = FindViewById<Spinner>(Resource.Id.spinnerWeight1);
            spinnerWeight2 = FindViewById<Spinner>(Resource.Id.spinnerWeight2);
            
            spinnerWeight1.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.Weight, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerWeight1.Adapter = adapter;

            spinnerWeight2.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            spinnerWeight2.Adapter = adapter;


            //Initialize all lists
            listBtnAddMtn = new List<Button>();
            listBtnAddMtn.Add(btnAddMtn1);
            listBtnAddMtn.Add(btnAddMtn2);

            listTextMontants = new List<EditText>();
            listTextMontants.Add(textMontant1);
            listTextMontants.Add(textMontant2);

            listTextTotaux = new List<EditText>();
            listTextTotaux.Add(textTotal1);
            listTextTotaux.Add(textTotal2);

            listPeoples = new List<EditText>();
            listPeoples.Add(textName1);
            listPeoples.Add(textName2);

            listSpinners = new List<Spinner>();
            listSpinners.Add(spinnerWeight1);
            listSpinners.Add(spinnerWeight2);

            montants = new List<float>();
            montants.Add(0);
            montants.Add(0);

            listChkTax = new List<CheckBox>();
            listChkTax.Add(chkTax1);
            listChkTax.Add(chkTax2);

            //add listeners on text changed to enable button add
            foreach (EditText text in listTextMontants)
            {
                text.TextChanged += TextMontant_TextChanged;
            }

            //add listeners on add to add montant to total
            foreach (Button btnAdd in listBtnAddMtn)
            {
                btnAdd.Click += BtnAddMtn_Click;
            }

            //add listeners on add people btn
            btnAddPeople.Click += BtnAddPeople_Click;

            btnCalculate.Click += btnCalculate_Click;

            NbPeople = 2;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            /*
            Spinner spinner = (Spinner)sender;
            int index = listSpinners.IndexOf(spinner);*/
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            //Clean the result before if it isn't the first calculate
            if (layoutResult != null)
            {
                layoutResult.RemoveAllViews();
            }
            layoutResult = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent)
            };

            //Grab the root of view, to add the result at the end
            LinearLayout layoutRoot = this.FindViewById<LinearLayout>(Resource.Id.linearRoot);

            float TotalGlobal = 0;

            //Calculate the total montant of all bills
            foreach (EditText totalText in listTextTotaux)
            {
                if (!String.IsNullOrWhiteSpace(totalText.Text))
                {
                    float total = float.Parse(totalText.Text);
                    TotalGlobal += total;
                }                
            }

            //Create the text view that will show the amount
            var textTotal = new TextView(this)
            {
                Text = "Total Global : " + TotalGlobal.ToString("c2")
            };

            layoutResult.AddView(textTotal);

            var textMoyenne = new TextView(this)
            {
                Text = "Moyenne des contributions : " + (TotalGlobal / (float)NbPeople).ToString("c2")
            };
            layoutResult.AddView(textMoyenne);

            //Calculate the amount du to each person
            var Weight = GetMaxWeight();
            int MaxWeight = Weight.Item1;
            int TotalWeight = Weight.Item2;

            for (int i = 0; i < listPeoples.Count; ++i)
            {
                float contribution = 0;
                if (!String.IsNullOrWhiteSpace(listTextTotaux[i].Text))
                {
                    contribution = float.Parse(listTextTotaux[i].Text);
                }
                else
                {
                    contribution = 0;
                }
                float poids = float.Parse(listSpinners[i].SelectedItem.ToString());
                float montantToPay = TotalGlobal * (poids / TotalWeight) - contribution;

                var textContribution = new TextView(this)
                {
                    Text = listPeoples[i].Text + " -> Montant à Balancer = " + (montantToPay).ToString("c2")
                };

                layoutResult.AddView(textContribution);
            }

            layoutRoot.AddView(layoutResult);
        }

        private Tuple<int, int> GetMaxWeight()
        {
            int TotalWeight = 0, MaxWeight = 0;
            foreach (Spinner spinner in listSpinners)
            {
                int weight = int.Parse(spinner.SelectedItem.ToString());
                if (weight > MaxWeight)
                {
                    MaxWeight = weight;
                }
                TotalWeight += weight;
            }
            return new Tuple<int, int>(MaxWeight, TotalWeight);
        }

        private void BtnAddPeople_Click(object sender, EventArgs e)
        {
            //Create a new line that correspond to a new people
            //So need to create all the buttons and labels corresponding to a people

            LinearLayout layoutPeople = this.FindViewById<LinearLayout>(Resource.Id.linearPeople);

            var layout1 = new LinearLayout(this)
            {
                Orientation = Orientation.Horizontal,
                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent)
            };
            var spinnerWeight = new Spinner(this)
            {
                
            };
            spinnerWeight.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.Weight, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerWeight.Adapter = adapter;
            var editTextName = new EditText(this)
            {
                Text = "Bob",
                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent)
            };

            layout1.AddView(spinnerWeight);
            layout1.AddView(editTextName);

            layoutPeople.AddView(layout1);

            var layout2 = new LinearLayout(this)
            {
                Orientation = Orientation.Horizontal,
                LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent)
            };
            var editMtn = new EditText(this)
            {
                Hint = "Add Montant",
                InputType = Android.Text.InputTypes.ClassNumber
            };
            var chkTax = new CheckBox(this)
            {
                Text = "+ Tax",
                Checked = true
            };
            var btnAdd = new Button(this)
            {
                Text = "Add",
                Enabled = false
            };
            var textlabel = new EditText(this)
            {
                Text = "Total:",
                Focusable = false                
            };
            var textTotal = new EditText(this)
            {
                Text = "",
                InputType = Android.Text.InputTypes.ClassNumber
            };
            layout2.AddView(editMtn);
            layout2.AddView(chkTax);
            layout2.AddView(btnAdd);
            layout2.AddView(textlabel);
            layout2.AddView(textTotal);

            editMtn.TextChanged += TextMontant_TextChanged;
            btnAdd.Click += BtnAddMtn_Click;

            listBtnAddMtn.Add(btnAdd);
            listChkTax.Add(chkTax);
            listTextMontants.Add(editMtn);
            listTextTotaux.Add(textTotal);
            listPeoples.Add(editTextName);
            listSpinners.Add(spinnerWeight);

            montants.Add(0);

            //Add it to the view  
            layoutPeople.AddView(layout2);

            ++NbPeople;
        }

        private void BtnAddMtn_Click(object sender, EventArgs e)
        {
            Button btnAdd = (Button)sender;
            int index = listBtnAddMtn.IndexOf(btnAdd);

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
            listBtnAddMtn[index].Enabled = false;
        }

        private void TextMontant_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            EditText textMontant = (EditText)sender;
            if (!String.IsNullOrWhiteSpace(textMontant.Text))
            {
                int index = listTextMontants.IndexOf(textMontant);
                montants[index] = float.Parse(textMontant.Text);
                listBtnAddMtn[index].Enabled = true;       
            }
        }


    }
}

