using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CurrencyConverter_Static
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con = new SqlConnection(); // Create Object for SqlConnection
        SqlCommand cmd = new SqlCommand(); // Create Object for SqlCommand
        SqlDataAdapter adapter = new SqlDataAdapter(); // Create Object for SqlAdapter

        private int CurrencyId = 0; // Declare CurrencyId with int datatype and assign value 0
        private double FromAmount = 0; // Declare FromAmount with double datatype and assign value 0
        private double ToAmount = 0; // Declare ToAmount with double datatype and assign value 0


        Root val = new Root();

        public MainWindow()
        {
            InitializeComponent();

            // clearcontrols method is used to clear all control values
            ClearControls();

            // BindCurrency is used to bind currency name with the value in the Combobox
            //BindCurrency();

            GetValue();

            //GetData();
        }
        // CRUD

        private async void GetValue()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=916e25b6913949518da95354a5c27df9");
            BindCurrency();
        }

        public static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();
            try
            {
                // HttpClient class provides a base class for sending/receiving the http request/response from a URl.
                using (var client = new HttpClient())
                {
                    // The time span to wait before the request times out.
                    client.Timeout = TimeSpan.FromMinutes(1);

                    // HttpsResponseMessage is a way of returning a  message/data from your action.
                    HttpResponseMessage response = await client.GetAsync(url);
                    // check API response code ok
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) 
                    {
                        // Serialize the http content to a string as an aysnchronius operation.
                        var ResponseString = await response.Content.ReadAsStringAsync();
                        // JsonConvert.DeserializeObject to deserialize Json to C#
                        var ResponseObject =JsonConvert.DeserializeObject<Root>(ResponseString);

                        MessageBox.Show("Rates: " + ResponseString, "Information", MessageBoxButton.OK, MessageBoxImage.Information); // If amount TextBox

                        return ResponseObject; //Return API response
                    }
                    return myRoot;
                } 
            }
            catch
            {

                return myRoot;
            }
        }

        public void mycon()
        {
            string Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; // DataBase Connection String
            con = new SqlConnection(Conn);
            con.Open(); //Connection open
        }

        private void BindCurrency()
        {
            // +API
            // Create an object for DataTable
            DataTable dt = new DataTable();

            // Add display column in DataTable
            dt.Columns.Add("Text");

            // Add the value column in the DataTable
            //dt.Columns.Add("Value");

            // Add value column in DataTable
            dt.Columns.Add("Rate");

            //Add rows in Datatable with text and value. set a value which are fetched from API
            dt.Rows.Add("--SELECT--", 0);
            dt.Rows.Add("CAD", val.rates.CAD);
            dt.Rows.Add("CZK", val.rates.CZK);
            dt.Rows.Add("DKK", val.rates.DKK);
            dt.Rows.Add("EUR", val.rates.EUR);
            dt.Rows.Add("INR", val.rates.INR);
            dt.Rows.Add("ISK", val.rates.ISK);
            dt.Rows.Add("JPY", val.rates.JPY);
            dt.Rows.Add("NGN", val.rates.NGN);
            dt.Rows.Add("NZD", val.rates.NZD);
            dt.Rows.Add("PHP", val.rates.PHP);
            dt.Rows.Add("USD", val.rates.USD);

            //Datatable data assign From currency Combobox
            cmbFromCurrency.ItemsSource = dt.DefaultView;

            //DisplayMemberPath property is used to display data in Combobox
            cmbFromCurrency.DisplayMemberPath = "Text";

            //SelectedValuePath property is used for set value in Combobox
            cmbFromCurrency.SelectedValuePath = "Rate";

            //SelectedIndex property is used to bind Combobox it's default selected item is first
            cmbFromCurrency.SelectedIndex = 0;

            //All property set to To Currency Combobox as From Currency Combobox
            cmbToCurrency.ItemsSource = dt.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Rate";
            cmbToCurrency.SelectedIndex = 0;

            /* // +DataBase
             * mycon();
            // Create an object for DataTable
            DataTable dt = new DataTable();
            // Write query to get data from Currency_Master table
            cmd = new SqlCommand("select Id, CurrencyName from Currency_Master", con);
            // CommandType define which type of commnd we use for write a query
            cmd.CommandType = CommandType.Text;

            // It is accepting a parameeter that contains the command text of the object's selectCommand property
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            // Create an object for DataRow
            DataRow newRow = dt.NewRow();
            // Assign a value to Id column
            newRow["Id"] = 0;
            // Assign value to CurrencyName column
            newRow["CurrencyName"] = "--SELECT--";

            // Insert a new row in dt with data at a 0 position
            dt.Rows.InsertAt(newRow, 0);

            // dt is ot null and rows count greater than 0
            if(dt != null && dt.Rows.Count > 0)
            {
                // Assign the datatable data to combobox using ItemSource property
                cmbFromCurrency.ItemsSource = dt.DefaultView;
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            con.Close();

            //The data to currency Combobox is assigned from datatable
            //Value Properties are used to display data in Combobox
            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Id";
            cmbFromCurrency.SelectedIndex = 0;

            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedIndex = 0;*/
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Declare ConvertedValue with double DataType for store currency converted value
            double ConvertedValue;

            //Check amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //If amount textbox is Null or Blank it will show the below message box   
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //After clicking on message box OK sets the Focus on amount textbox
                txtCurrency.Focus();
                return;
            }
            //Else if the currency from is not selected or it is default text --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                //It will show the message
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            //Else if Currency To is not Selected or Select Default Text --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                //It will show the message
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on To Combobox
                cmbToCurrency.Focus();
                return;
            }

            //If From and To Combobox selected values are same
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //The amount textbox value set in ConvertedValue.
                //double.parse is used to convert datatype String To Double.
                //Textbox text have string and ConvertedValue is double datatype
                ConvertedValue = double.Parse(txtCurrency.Text);

                //Show in label converted currency and converted currency name.
                // and ToString("N3") is used to place 000 after after the(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {

                //Calculation for currency converter is From Currency value multiply(*) 
                // with amount textbox value and then the total is divided(/) with To Currency value
                ConvertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString());

                //Show in label converted currency and converted currency name.
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        //Clear button click event
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //ClearControls method  is used to clear all control value
            ClearControls();
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e) //Allow Only Integer in Text Box
        {
            //Regular Expression is used to add regex.
             //Add Library using System.Text.RegularExpressions;
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtAmount.Text == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter currency name","Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus(); 
                    return;
                }
                else
                {   // for when the currency ID is greater than zero
                    if (CurrencyId > 0) // Code for Update button. Here check CurrencyId greater than zero  than it is go for update
                    {
                        if (MessageBox.Show("Are you sure you want to update?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) // Show confirmation message
                        {
                            mycon();
                            DataTable dt = new DataTable();
                            cmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = CurrencyName WHERE Id = @Id", con); // Update query record Update using Id
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
                            cmd.Parameters.AddWithValue("@Amount", txtAmount);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                    }   // for when the currency ID is not greater than zero - we have to make sure we add a new line
                    else // Save Button 
                    {
                        if (MessageBox.Show("Are you sure you want to save ?","Information", MessageBoxButton.YesNo, MessageBoxImage.Question ) == MessageBoxResult.Yes)
                        {
                            mycon();
                            cmd = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", con); // Insert Query for Save data in  the Table
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data saved successfully", "Informartion", MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                    }
                    ClearMaster();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); 
                return;
            }
        }

        private void ClearMaster() //This Method is Used to Clear All the input Which User Entered in Currency Master tab
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                CurrencyId = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch (Exception ex)    
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void GetData() // Bind Data in DataGrid View.
        {
            mycon();    // mycon() method is used for connect with database and open database connection
            DataTable dt = new DataTable();     // Create DataTable object
            cmd = new SqlCommand("SELECT * FROM Currency_Master", con);     // Write Sql Query for Get data from databse table.
            cmd.CommandType = CommandType.Text;     // CommandType define which type of command execute like Text, StoredProcedure
            /// SqlDataAdapter adapter = new SqlDataAdapter(cmd);      //  It is accept a parameter that contains the command text of the object's SelectCommand
            adapter = new SqlDataAdapter(cmd);      //  It is accept a parameter that contains the command text of the object's SelectCommand
            adapter.Fill(dt);       // The DataAdapter serves as a bridge between a DataSet and a data source for retrieving and saving
            if (dt != null && dt.Rows.Count > 0)        // dt is not null and rows count greater than 0
                dgvCurrency.ItemsSource = dt.DefaultView;       // Assign DataTable dataTable data to dgvCurrency using ItemSource Property.
            else
                dgvCurrency.ItemsSource = null;
            con.Close();        // Database connection Close
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid grd = (DataGrid)sender; // Object for datagrid
                DataRowView row_selected = grd.CurrentItem as DataRowView; // Object for datarowview

                if (row_selected != null)
                {
                    if (dgvCurrency.Items.Count > 0)
                    {
                        if (grd.SelectedCells.Count > 0)
                        {
                            CurrencyId = Int32.Parse(row_selected["Id"].ToString());

                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)
                            {
                                txtAmount.Text = row_selected["Amount"].ToString();
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();
                                btnSave.Content = "Update";
                            }
                            if (grd.SelectedCells[0].Column.DisplayIndex == 1)
                            {
                                if (MessageBox.Show("Are you sure you want to delete ?", "Informaton", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    mycon();
                                    DataTable dt = new DataTable();
                                    cmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", con);
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@Id", CurrencyId);
                                    cmd.ExecuteNonQuery();
                                    con.Close();

                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMaster();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}