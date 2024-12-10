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


    // زر لتحديث البيانات
    let refreshButton = new Button(Text = "Refresh Data", Top = 230, Left = 220, Width = 120)


    // دالة لجلب البيانات من قاعدة البيانات وملء الجدول
    let loadData () =
        try
            use connection = new MySqlConnection(connectionString)
            connection.Open()

            let query = "SELECT ID, name, grades ,English,CS,FS FROM studen_info"
            use adapter = new MySqlDataAdapter(query, connection)
            let dataTable = new DataTable()
            adapter.Fill(dataTable) |> ignore

            studentGridView.DataSource <- dataTable // ربط البيانات بالجدول
        with
        | ex -> MessageBox.Show("Error loading data: " + ex.Message) |> ignore


        // تحديث البيانات عند الضغط على زر "Refresh Data"
        refreshButton.Click.Add(fun _ -> loadData())


        // إنشاء الحقول
        let nameLabel = new Label(Text = "Name:", AutoSize = true, Top =300, Left = 20)
        let nameTextBox = new TextBox(Width = 200, Top =300, Left = 80)


        let IDLabel = new Label(Text = "ID:", AutoSize = true, Top =260, Left = 20)
        let IDTextBox = new TextBox(Width = 200, Top =260, Left = 80)

        let gradesTextBox = new TextBox(Width = 200, Top = 300, Left = 80)

        let EnglishLabel = new Label(Text = "English:", AutoSize = true, Top = 330, Left = 320)
        let EnglishTextBox = new TextBox(Width = 200, Top = 330, Left = 380)

        let CSLabel = new Label(Text = "CS:", AutoSize = true, Top = 260, Left = 320)
        let CSTextBox = new TextBox(Width = 200, Top = 260, Left = 380)

        let FSLabel = new Label(Text = "FS:", AutoSize = true, Top = 300, Left = 320)
        let FSTextBox = new TextBox(Width = 200, Top = 300, Left = 380)




// زر "Search"
 let searchLabel = new Label(Text = "Search:", AutoSize = true, Top = 400, Left = 20)
 let searchTextBox = new TextBox(Width = 200, Top = 400, Left = 80)

 let searchButton = new Button(Text = "Search", Top = 400, Left = 300, Width = 100)
 searchButton.Click.Add(fun _ ->
     try
         let searchValue = searchTextBox.Text
         if String.IsNullOrWhiteSpace(searchValue) then
             MessageBox.Show("Please enter a ID to search.") |> ignore
         else
             use connection = new MySqlConnection(connectionString)
             connection.Open()

             let query = "SELECT ID, name, grades ,English , CS , FS FROM studen_info WHERE ID = @ID"
             use command = new MySqlCommand(query, connection)
             command.Parameters.AddWithValue("@ID", Int32.Parse(searchValue)) |> ignore

             use reader = command.ExecuteReader()

             if reader.Read() then
                IDTextBox.Text <- reader.GetInt32(0).ToString()
                nameTextBox.Text <- reader.GetString(1)
                gradesTextBox.Text <- reader.GetInt32(2).ToString()
                EnglishTextBox.Text <- reader.GetInt32(3).ToString()
                CSTextBox.Text <- reader.GetInt32(4).ToString()
                FSTextBox.Text <- reader.GetInt32(5).ToString()

             else
                 MessageBox.Show("Not found any data about this input.") |> ignore

             reader.Close()
     with
     | ex -> MessageBox.Show($"Error: {ex.Message}") |> ignore
 )



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
