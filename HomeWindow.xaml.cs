using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using MyApp.Interface.Models;
using MyApp.Interface.Services;

namespace MyApp.Interface
{
    public partial class HomeWindow : Window
    {
        private readonly UserSession _session;
        private bool _isDarkTheme = false;

        public HomeWindow(UserSession session)
        {
            InitializeComponent();
            _session = session;
            lblWelcome.Text = $"Bienvenue, {session.Username} !";
            LoadDataToUi();
            ApplyPermissions();
            LoadDashboardStats();
            LoadRecentGrades();
        }

        private void LoadDataToUi()
        {
            dgStudents.ItemsSource = InMemoryStore.Students;
            cbStudentClass.ItemsSource = InMemoryStore.Classes;
            cbAssignClass.ItemsSource = InMemoryStore.Classes;
            lbAssignSubjects.ItemsSource = InMemoryStore.Subjects;
            dgClasses.ItemsSource = InMemoryStore.Classes;
            dgSubjects.ItemsSource = InMemoryStore.Subjects;
            dgTeachers.ItemsSource = InMemoryStore.Teachers;
            dgGrades.ItemsSource = InMemoryStore.Grades;
            cbGradeStudent.ItemsSource = InMemoryStore.Students;
            cbGradeSubject.ItemsSource = InMemoryStore.Subjects;

            RefreshAverages();
        }

        // helper pour récupérer Guid? depuis ComboBox.SelectedValue
        private Guid? GetSelectedGuid(ComboBox cb)
        {
            if (cb == null) return null;
            if (cb.SelectedValue is Guid g) return g;
            if (cb.SelectedValue is Guid?) return (Guid?)cb.SelectedValue;
            return null;
        }

        #region Etudiants CRUD & recherche

        private void BtnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            var s = new Student
            {
                FirstName = txtStudentFirstName.Text ?? string.Empty,
                LastName = txtStudentLastName.Text ?? string.Empty,
                Address = txtStudentAddress.Text ?? string.Empty,
                Contact = txtStudentContact.Text ?? string.Empty,
                ParentsNames = txtStudentParents.Text ?? string.Empty,
                PhotoPath = txtStudentPhotoPath.Text ?? string.Empty,
                DateOfBirth = dpStudentDob.SelectedDate ?? DateTime.Today.AddYears(-15),
                ClassId = GetSelectedGuid(cbStudentClass)
            };
            InMemoryStore.Students.Add(s);
            ClearStudentForm();
            RefreshAverages();
            InMemoryStore.Save(); // Save changes
        }

        private void BtnUpdateStudent_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student s)
            {
                s.FirstName = txtStudentFirstName.Text;
                s.LastName = txtStudentLastName.Text;
                s.Address = txtStudentAddress.Text;
                s.Contact = txtStudentContact.Text;
                s.ParentsNames = txtStudentParents.Text;
                s.PhotoPath = txtStudentPhotoPath.Text;
                s.DateOfBirth = dpStudentDob.SelectedDate ?? s.DateOfBirth;
                s.ClassId = GetSelectedGuid(cbStudentClass);
                dgStudents.Items.Refresh();
                RefreshAverages();
                InMemoryStore.Save(); // Save changes
            }
        }

        private void BtnDeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student s)
            {
                var gradesToRemove = InMemoryStore.Grades.Where(g => g.StudentId == s.Id).ToList();
                foreach (var g in gradesToRemove) InMemoryStore.Grades.Remove(g);

                InMemoryStore.Students.Remove(s);
                ClearStudentForm();
                RefreshAverages();
                InMemoryStore.Save(); // Save changes
            }
        }

        private void ClearStudentForm()
        {
            txtStudentFirstName.Clear();
            txtStudentLastName.Clear();
            txtStudentAddress.Clear();
            txtStudentContact.Clear();
            txtStudentParents.Clear();
            txtStudentPhotoPath.Text = string.Empty;
            imgStudentPreview.Source = null;
            dpStudentDob.SelectedDate = DateTime.Today.AddYears(-15);
            cbStudentClass.SelectedIndex = -1;
        }

        private void BtnBrowseStudentPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Images (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png" };
            if (dlg.ShowDialog() == true)
            {
                txtStudentPhotoPath.Text = dlg.FileName;
                try 
                { 
                    imgStudentPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(dlg.FileName)); 
                }
                catch { }
            }
        }

        private void DgStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student s)
            {
                txtStudentFirstName.Text = s.FirstName;
                txtStudentLastName.Text = s.LastName;
                txtStudentAddress.Text = s.Address;
                txtStudentContact.Text = s.Contact;
                txtStudentParents.Text = s.ParentsNames;
                txtStudentPhotoPath.Text = s.PhotoPath;
                if (!string.IsNullOrEmpty(s.PhotoPath) && File.Exists(s.PhotoPath))
                {
                     try { imgStudentPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(s.PhotoPath)); } catch {}
                }
                else imgStudentPreview.Source = null;

                dpStudentDob.SelectedDate = s.DateOfBirth;
                cbStudentClass.SelectedValue = s.ClassId;
            }
        }

        private void TxtSearchStudent_TextChanged(object sender, TextChangedEventArgs e)
        {
            var q = txtSearchStudent.Text?.Trim().ToLower() ?? string.Empty;
            var view = CollectionViewSource.GetDefaultView(dgStudents.ItemsSource) as CollectionView;
            if (view != null)
            {
                view.Filter = o =>
                {
                    if (o is Student s)
                    {
                        var className = InMemoryStore.ClassNameFor(s.ClassId).ToLower();
                        return s.FirstName.ToLower().Contains(q) || 
                               s.LastName.ToLower().Contains(q) || 
                               s.Matricule.ToLower().Contains(q) || // Search by Matricule
                               className.Contains(q);
                    }
                    return false;
                };
                view.Refresh();
            }
        }

        #endregion

        #region Enseignants CRUD

        private void BtnAddTeacher_Click(object sender, RoutedEventArgs e)
        {
            InMemoryStore.Teachers.Add(new Teacher
            {
                Name = txtTeacherName.Text,
                Specialty = txtTeacherSpecialty.Text,
                Subjects = txtTeacherSubjects.Text,
                PhotoPath = txtTeacherPhotoPath.Text
            });
            ClearTeacherForm();
            InMemoryStore.Save();
        }

        private void BtnUpdateTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (dgTeachers.SelectedItem is Teacher t)
            {
                t.Name = txtTeacherName.Text;
                t.Specialty = txtTeacherSpecialty.Text;
                t.Subjects = txtTeacherSubjects.Text;
                t.PhotoPath = txtTeacherPhotoPath.Text;
                dgTeachers.Items.Refresh();
                InMemoryStore.Save();
            }
        }

        private void BtnDeleteTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (dgTeachers.SelectedItem is Teacher t) InMemoryStore.Teachers.Remove(t);
            ClearTeacherForm();
            InMemoryStore.Save();
        }

        private void ClearTeacherForm()
        {
            txtTeacherName.Clear();
            txtTeacherSpecialty.Clear();
            txtTeacherSubjects.Clear();
            txtTeacherPhotoPath.Text = string.Empty;
            imgTeacherPreview.Source = null;
        }

        private void BtnBrowseTeacherPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Images (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png" };
            if (dlg.ShowDialog() == true)
            {
                txtTeacherPhotoPath.Text = dlg.FileName;
                try 
                { 
                    imgTeacherPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(dlg.FileName)); 
                }
                catch { }
            }
        }

        private void DgTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTeachers.SelectedItem is Teacher t)
            {
                txtTeacherName.Text = t.Name;
                txtTeacherSpecialty.Text = t.Specialty;
                txtTeacherSubjects.Text = t.Subjects;
                txtTeacherPhotoPath.Text = t.PhotoPath;
                if (!string.IsNullOrEmpty(t.PhotoPath) && File.Exists(t.PhotoPath))
                {
                     try { imgTeacherPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(t.PhotoPath)); } catch {}
                }
                else imgTeacherPreview.Source = null;
            }
        }

        private void TxtSearchTeacher_TextChanged(object sender, TextChangedEventArgs e)
        {
            var q = txtSearchTeacher.Text?.Trim().ToLower() ?? string.Empty;
            var view = CollectionViewSource.GetDefaultView(dgTeachers.ItemsSource) as CollectionView;
            if (view != null)
            {
                view.Filter = o =>
                {
                    if (o is Teacher t)
                    {
                        return t.Name.ToLower().Contains(q) || 
                               t.Matricule.ToLower().Contains(q) || // Search by Matricule
                               t.Subjects.ToLower().Contains(q);
                    }
                    return false;
                };
                view.Refresh();
            }
        }

        #endregion

        #region Classes & Matières

        private void BtnAddClass_Click(object sender, RoutedEventArgs e)
        {
            InMemoryStore.Classes.Add(new SchoolClass { Name = txtClassName.Text, AssignedSubjects = txtClassSubjects.Text });
            dgClasses.Items.Refresh();
            cbStudentClass.Items.Refresh();
            cbAssignClass.Items.Refresh();
            InMemoryStore.Save();
        }

        private void BtnUpdateClass_Click(object sender, RoutedEventArgs e)
        {
            if (dgClasses.SelectedItem is SchoolClass c)
            {
                c.Name = txtClassName.Text;
                c.AssignedSubjects = txtClassSubjects.Text;
                dgClasses.Items.Refresh();
                cbStudentClass.Items.Refresh();
                cbAssignClass.Items.Refresh();
                InMemoryStore.Save();
            }
        }

        private void BtnDeleteClass_Click(object sender, RoutedEventArgs e)
        {
            if (dgClasses.SelectedItem is SchoolClass c) InMemoryStore.Classes.Remove(c);
            dgClasses.Items.Refresh();
            cbStudentClass.Items.Refresh();
            cbAssignClass.Items.Refresh();
            InMemoryStore.Save();
        }

        private void BtnAddSubject_Click(object sender, RoutedEventArgs e)
        {
            InMemoryStore.Subjects.Add(new Subject { Name = txtSubjectName.Text });
            dgSubjects.Items.Refresh();
            lbAssignSubjects.Items.Refresh();
            cbGradeSubject.Items.Refresh();
            InMemoryStore.Save();
        }

        private void BtnDeleteSubject_Click(object sender, RoutedEventArgs e)
        {
            if (dgSubjects.SelectedItem is Subject s) InMemoryStore.Subjects.Remove(s);
            dgSubjects.Items.Refresh();
            lbAssignSubjects.Items.Refresh();
            InMemoryStore.Save();
        }

        private void BtnAssignSubjects_Click(object sender, RoutedEventArgs e)
        {
            if (cbAssignClass.SelectedItem is SchoolClass c)
            {
                var selected = lbAssignSubjects.SelectedItems.Cast<Subject>().Select(x => x.Name);
                c.AssignedSubjects = string.Join(",", selected);
                dgClasses.Items.Refresh();
                InMemoryStore.Save();
            }
        }

        private void DgClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgClasses.SelectedItem is SchoolClass c)
            {
                txtClassName.Text = c.Name;
                txtClassSubjects.Text = c.AssignedSubjects;
            }
        }

        private void DgSubjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSubjects.SelectedItem is Subject s) txtSubjectName.Text = s.Name;
        }

        #endregion

        #region Notes & Moyennes

        private void BtnAddGrade_Click(object sender, RoutedEventArgs e)
        {
            if (cbGradeStudent.SelectedValue is Guid studentId && cbGradeSubject.SelectedValue is Guid subjectId)
            {
                if (double.TryParse(txtGradeValue.Text, out var v))
                {
                    if (v < 0 || v > 20) { MessageBox.Show("Note doit être entre 0 et 20."); return; }
                    InMemoryStore.Grades.Add(new Grade { StudentId = studentId, SubjectId = subjectId, Value = v });
                    dgGrades.Items.Refresh();
                    dgGrades.Items.Refresh();
                    RefreshAverages();
                    InMemoryStore.Save();
                }
                else MessageBox.Show("Valeur de note invalide.");
            }


            else
            {
                MessageBox.Show("Sélectionnez étudiant et matière.");
            }
        }

        private void BtnThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            _isDarkTheme = !_isDarkTheme;
            
            if (sender is Button btn)
            {
                btn.Content = _isDarkTheme ? "☀️" : "🌙";
            }

            var res = Application.Current.Resources;

            if (_isDarkTheme)
            {
                res["PageBackground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                res["BackgroundBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                res["TextDarkBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                res["TextLightBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#94A3B8"));
            }
            else
            {
                res["PageBackground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                res["BackgroundBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                res["TextDarkBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                res["TextLightBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4B5563"));
            }
        }

        private void RefreshAverages()
        {
            var avgs = InMemoryStore.Students.Select(s =>
            {
                var gs = InMemoryStore.Grades.Where(g => g.StudentId == s.Id).Select(g => g.Value).ToList();
                var avg = gs.Any() ? gs.Average() : 0.0;
                return new { StudentName = s.FullName, Average = Math.Round(avg, 2) };
            }).ToList();

            dgAverages.ItemsSource = avgs;
            dgAverages.Items.Refresh();
        }

        private void BtnGenerateBulletin_Click(object sender, RoutedEventArgs e)
        {
            if (cbGradeStudent.SelectedValue is Guid studentId)
            {
                var student = InMemoryStore.Students.FirstOrDefault(s => s.Id == studentId);
                if (student == null) { MessageBox.Show("Étudiant non trouvé."); return; }

                var grades = InMemoryStore.Grades.Where(g => g.StudentId == studentId).ToList();
                var avg = grades.Any() ? grades.Average(x => x.Value) : 0.0;
                var className = InMemoryStore.ClassNameFor(student.ClassId);

                // HTML Construction
                var html = $@"
<!DOCTYPE html>
<html lang='fr'>
<head>
    <meta charset='UTF-8'>
    <title>Bulletin - {student.FullName}</title>
    <style>
        body {{ font-family: 'Segoe UI', sans-serif; padding: 40px; background: #f3f4f6; }}
        .container {{ max-width: 800px; margin: 0 auto; background: white; padding: 40px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
        h1 {{ color: #1e293b; border-bottom: 2px solid #2563eb; padding-bottom: 10px; }}
        .header {{ display: flex; justify-content: space-between; margin-bottom: 30px; }}
        .info p {{ margin: 5px 0; color: #475569; }}
        table {{ width: 100%; border-collapse: collapse; margin-bottom: 30px; }}
        th, td {{ padding: 12px; text-align: left; border-bottom: 1px solid #e2e8f0; }}
        th {{ background: #f8fafc; color: #1e293b; }}
        .total {{ font-size: 1.2em; font-weight: bold; text-align: right; margin-top: 20px; color: #1e293b; }}
        .grade {{ font-weight: bold; color: #2563eb; }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Bulletin de Notes</h1>
        <div class='header'>
            <div class='info'>
                <p><strong>Nom:</strong> {student.LastName}</p>
                <p><strong>Prénom:</strong> {student.FirstName}</p>
                <p><strong>Matricule:</strong> {student.Matricule}</p>
            </div>
            <div class='info'>
                <p><strong>Classe:</strong> {className}</p>
                <p><strong>Date:</strong> {DateTime.Now:d}</p>
            </div>
        </div>

        <table>
            <thead><tr><th>Matière</th><th>Note</th><th>Date</th></tr></thead>
            <tbody>";

                foreach (var g in grades)
                {
                    var subj = InMemoryStore.SubjectName(g.SubjectId);
                    html += $"<tr><td>{subj}</td><td class='grade'>{g.Value}/20</td><td>{g.Date:d}</td></tr>";
                }

                html += $@"
            </tbody>
        </table>

        <div class='total'>Moyenne Générale: {avg:F2}/20</div>
    </div>
</body>
</html>";

                var dlg = new SaveFileDialog { FileName = $"{student.LastName}_{student.FirstName}_bulletin.html", Filter = "HTML File|*.html" };
                if (dlg.ShowDialog() == true)
                {
                    File.WriteAllText(dlg.FileName, html);
                    
                    try
                    {
                        // Open in browser
                        var p = new System.Diagnostics.Process();
                        p.StartInfo = new System.Diagnostics.ProcessStartInfo(dlg.FileName) { UseShellExecute = true };
                        p.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fichier enregistré, mais impossible d'ouvrir le navigateur : " + ex.Message);
                    }
                }
            }
            else MessageBox.Show("Sélectionnez un étudiant en haut à gauche.");
        }

        #endregion

        #region Exports / Impression (simulés)

        private void BtnExportStudents_Click(object sender, RoutedEventArgs e) => ExportStudentsCsv();
        private void BtnExportTeachers_Click(object sender, RoutedEventArgs e) => ExportTeachersCsv();
        private void BtnExportGrades_Click(object sender, RoutedEventArgs e) => ExportGradesCsv();

        private void ExportStudentsCsv()
        {
            var dlg = new SaveFileDialog { FileName = "students.csv", Filter = "CSV|*.csv" };
            if (dlg.ShowDialog() != true) return;
            using var sw = new StreamWriter(dlg.FileName);
            sw.WriteLine("Id,LastName,FirstName,DateOfBirth,Address,Contact,Class");
            foreach (var s in InMemoryStore.Students)
            {
                sw.WriteLine($"{s.Id},{s.LastName},{s.FirstName},{s.DateOfBirth:d},{s.Address},{s.Contact},{InMemoryStore.ClassNameFor(s.ClassId)}");
            }
            MessageBox.Show($"Export étudiants enregistré : {dlg.FileName}");
        }

        private void ExportTeachersCsv()
        {
            var dlg = new SaveFileDialog { FileName = "teachers.csv", Filter = "CSV|*.csv" };
            if (dlg.ShowDialog() != true) return;
            using var sw = new StreamWriter(dlg.FileName);
            sw.WriteLine("Id,Name,Specialty,Subjects");
            foreach (var t in InMemoryStore.Teachers)
            {
                sw.WriteLine($"{t.Id},{t.Name},{t.Specialty},{t.Subjects}");
            }
            MessageBox.Show($"Export enseignants enregistré : {dlg.FileName}");
        }

        private void ExportGradesCsv()
        {
            var dlg = new SaveFileDialog { FileName = "grades.csv", Filter = "CSV|*.csv" };
            if (dlg.ShowDialog() != true) return;
            using var sw = new StreamWriter(dlg.FileName);
            sw.WriteLine("Id,Student,Subject,Value,Date");
            foreach (var g in InMemoryStore.Grades)
            {
                sw.WriteLine($"{g.Id},{InMemoryStore.StudentName(g.StudentId)},{InMemoryStore.SubjectName(g.SubjectId)},{g.Value},{g.Date:d}");
            }
            MessageBox.Show($"Export notes enregistré : {dlg.FileName}");
        }

        private void BtnExportAll_Click(object sender, RoutedEventArgs e)
        {
            ExportStudentsCsv();
            ExportTeachersCsv();
            ExportGradesCsv();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Impression simulée : utilisez Export CSV ou Générer bulletin pour créer des fichiers imprimables.", "Impression");
        }

        #endregion

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo(6, "À Propos", "Informations sur l'application");
        }

        #region Navigation Sidebar

        private void BtnNavDashboard_Click(object sender, RoutedEventArgs e) => NavigateTo(0, "Tableau de bord", "Aperçu des activités");
        private void BtnNavStudents_Click(object sender, RoutedEventArgs e) => NavigateTo(1, "Gestion des Étudiants", "Annuaire et suivi");
        private void BtnNavTeachers_Click(object sender, RoutedEventArgs e) => NavigateTo(2, "Gestion des Enseignants", "Personnel enseignant");
        private void BtnNavClasses_Click(object sender, RoutedEventArgs e) => NavigateTo(3, "Classes & Matières", "Organisation pédagogique");
        private void BtnNavGrades_Click(object sender, RoutedEventArgs e) => NavigateTo(4, "Notes & Bulletins", "Évaluations des élèves");
        private void BtnNavReports_Click(object sender, RoutedEventArgs e) => NavigateTo(5, "Rapports", "Exports et impressions");
        private void BtnNavAbout_Click(object sender, RoutedEventArgs e) => NavigateTo(6, "À Propos", "Informations sur l'application");

        private void NavigateTo(int index, string title, string subtitle)
        {
            MainTabControl.SelectedIndex = index;
            if (lblPageTitle != null) lblPageTitle.Text = title;
            if (lblPageSubtitle != null) lblPageSubtitle.Text = subtitle;
        }

        #endregion

        #region Permissions Update

        private void ApplyPermissions()
        {
            // Par défaut tout est visible (Admin)
            if (_session.Role == UserRole.Admin) return;

            // Masquer les onglets ET les boutons pour Enseignant et Etudiant
            TabStudents.Visibility = Visibility.Collapsed;
            TabTeachers.Visibility = Visibility.Collapsed;
            TabClasses.Visibility = Visibility.Collapsed;
            TabReports.Visibility = Visibility.Collapsed;

            // Masquer les boutons de navigation correspondants
            if (BtnNavStudents != null) BtnNavStudents.Visibility = Visibility.Collapsed;
            if (BtnNavTeachers != null) BtnNavTeachers.Visibility = Visibility.Collapsed;
            if (BtnNavClasses != null) BtnNavClasses.Visibility = Visibility.Collapsed;
            if (BtnNavReports != null) BtnNavReports.Visibility = Visibility.Collapsed;

            // Gestion spécifique
            if (_session.Role == UserRole.Student)
            {
                PanelGradeEntry.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Restored Methods

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Vous êtes sur le point de vous déconnecter.\nÊtes-vous sûr ?",
                "Déconnexion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var login = new MainWindow();
                login.Show();
                this.Close();
            }
        }

        private void LoadDashboardStats()
        {
            lblTotalStudents.Text = InMemoryStore.Students.Count.ToString();
            lblTotalTeachers.Text = InMemoryStore.Teachers.Count.ToString();
            lblTotalClasses.Text = InMemoryStore.Classes.Count.ToString();
        }

        private void LoadRecentGrades()
        {
            // Charge les dernières notes (Top 10)
            var recent = InMemoryStore.Grades
                .OrderByDescending(g => g.Date)
                .Take(10)
                .Select(g => new 
                { 
                    StudentName = InMemoryStore.StudentName(g.StudentId),
                    SubjectName = InMemoryStore.SubjectName(g.SubjectId),
                    Value = g.Value,
                    Date = g.Date
                })
                .ToList();
            
            dgRecentGrades.ItemsSource = recent;
        }

        #endregion
    }
}