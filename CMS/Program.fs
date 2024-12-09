open System
open System.Windows.Forms
open System.Data
open MySql.Data.MySqlClient

let connectionString = "Server=localhost;Database=student;User Id=root;Password=;"

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
