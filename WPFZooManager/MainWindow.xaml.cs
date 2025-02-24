 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WPFZooManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["WPFZooManager.Properties.Settings.ManagementZoodbConnectionString1"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            ShowZoos();
            ShowAnimals();
        }

        private void ShowZoos()
        {
            try
            {
                string qurey = "select * from Zoo";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(qurey, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();
                    sqlDataAdapter.Fill(zooTable);

                    listZoos.DisplayMemberPath = "Location";
                    listZoos.SelectedValuePath = "Id";
                    listZoos.ItemsSource = zooTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }

        private void ShowAssociatedAnimals()
        {
            try
            {
                string qurey = "select * from Animal a inner join ZooAnimal za on a.Id = za.AnimalId where za.ZooId = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(qurey, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                    DataTable animalTabel = new DataTable();

                    sqlDataAdapter.Fill(animalTabel);

                    listAssociatedAnimals.DisplayMemberPath = "Name";
                    listAssociatedAnimals.SelectedValuePath = "Id";
                    listAssociatedAnimals.ItemsSource = animalTabel.DefaultView;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
           
        }

        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
        }


        private void ShowAnimals()
        {
            try
            {
                string query = "select * from Animal";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    listAllAnimals.DisplayMemberPath = "Name";
                    listAllAnimals.SelectedValue = "Id";
                    listAllAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }

        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Zoo where id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
           
        }
        private void AddZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Zoo values (@Location)";
                SqlCommand sqlCommad = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommad.Parameters.AddWithValue("@Location", txtBox_Add.Text);
                sqlCommad.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }
        private void Add_Animal_To_Zoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate selection
                if (listZoos.SelectedItem == null || listAllAnimals.SelectedItem == null)
                {
                    MessageBox.Show("Please select a zoo and an animal.");
                    return;
                }

                // Extract values from DataRowView
                DataRowView selectedZoo = listZoos.SelectedItem as DataRowView;
                DataRowView selectedAnimal = listAllAnimals.SelectedItem as DataRowView;

                // Assuming the ID column in both lists is named "id"
                int zooId = Convert.ToInt32(selectedZoo["id"]);
                int animalId = Convert.ToInt32(selectedAnimal["id"]);

                string query = "INSERT INTO ZooAnimal (ZooId, AnimalId) VALUES (@ZooId, @AnimalId)";
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@ZooId", zooId);
                    sqlCommand.Parameters.AddWithValue("@AnimalId", animalId);

                    // ExecuteNonQuery is more appropriate for INSERT statements
                    int rowsAffected = sqlCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        MessageBox.Show("Animal successfully added to the zoo.");
                    else
                        MessageBox.Show("Error adding the animal.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals(); // Refresh the list after insertion
            }

        }
        private void RemoveAnimal_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DeleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listAllAnimals.SelectedItem == null)
                {
                    MessageBox.Show("Please select an animal!");
                    return;
                }

                DataRowView selectedRow = listAllAnimals.SelectedItem as DataRowView;
                int animalId = Convert.ToInt32(selectedRow["id"]);

                string query = "DELETE FROM Animal WHERE id = @AnimalId";
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@AnimalId", animalId);
                    int rowsAffected = sqlCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        MessageBox.Show("Animal is deleted successfully!");
                    else
                        MessageBox.Show("Error!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals(); 
            }

        }


        private void Add_Animal_Zoo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
        {

        }
       
    }
}
