open System
open System.Windows.Forms
open System.Data
open MySql.Data.MySqlClient

let connectionString = "Server=localhost;Database=student;User Id=root;Password=;"


//نافذة إدخال كلمة المرور
let showPasswordPrompt () =
    let passwordForm = new Form(Text = "Enter Password", Width = 300, Height = 150, StartPosition = FormStartPosition.CenterParent)
    let passwordLabel: Label = new Label(Text = "Password:", Top = 20, Left = 20, AutoSize = true)
    let passwordTextBox: TextBox = new TextBox(Top = 20, Left = 100, Width = 150, PasswordChar = '*')
    let okButton: Button = new Button(Text = "OK", Top = 60, Left = 60, Width = 80)
    let cancelButton: Button = new Button(Text = "Cancel", Top = 60, Left = 160, Width = 80)
    let mutable result: string option = None
    okButton.Click.Add(fun _ -> result <- Some passwordTextBox.Text; passwordForm.Close())
    cancelButton.Click.Add(fun _ -> result <- None; passwordForm.Close())
    passwordForm.Controls.AddRange([| passwordLabel :> Control; passwordTextBox :> Control; okButton :> Control; cancelButton :> Control |])
    passwordForm.ShowDialog() |> ignore
    result


// نافذة الإدمن (CRUD) مع زر العودة
let createAdminForm (mainForm: Form) =
    let form = new Form(Text = "Admin Panel", Width = 620, Height = 600)
    let backButton: Button = new Button(Text = "Back to Main", Top = 500, Left = 400, Width = 120)


     // إنشاء جدول لعرض البيانات
    let studentGridView = new DataGridView(Top = 20, Left = 20, Width = 550, Height = 200)
    studentGridView.AutoSizeColumnsMode <- DataGridViewAutoSizeColumnsMode.Fill


/////////////////////// saif 

/////////////////////// Maghol


////////////////////// abosera3


////////////////////// Abdelrahman

////////////////////// Aboubakr

////////////////////// Ahmed


// نافذة البداية (Main Menu)
let createMainForm () =
    let form = new Form(Text = "Main Menu", Width = 400, Height = 300)
    let adminButton: Button = new Button(Text = "Admin", Top = 50, Left = 140, Width = 100)
    let studentButton: Button = new Button(Text = "Student", Top = 120, Left = 140, Width = 100)

    adminButton.Click.Add(fun _ -> 
        match showPasswordPrompt () with 
        | Some password when password = "M2003" -> 
            let adminForm = createAdminForm form // تمرير نافذة البداية
            adminForm.Show()
            form.Hide()
        | Some _ -> 
            MessageBox.Show("Invalid Password!") |> ignore
        | None -> ()
    )

    studentButton.Click.Add(fun _ -> 
        let studentForm = createStudentForm form // تمرير نافذة البداية
        studentForm.Show()
        form.Hide()
    )

    form.Controls.AddRange([| adminButton :> Control; studentButton :> Control |])
    form

[<EntryPoint>]
let main argv =
        let mainForm = createMainForm()
        Application.Run(mainForm)
        0
