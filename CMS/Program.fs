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



// زر "Add"
 let addButton = new Button(Text = "Add", Top = 360, Left = 300, Width = 100)
 addButton.Click.Add(fun _ ->
     try
         let name = nameTextBox.Text
 
         let ID = IDTextBox.Text
         let English = EnglishTextBox.Text
         let CS = CSTextBox.Text
         let FS = FSTextBox.Text



         if String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(ID) then
             MessageBox.Show("Please fill all fields.") |> ignore
         elif  Int32.Parse(English) > 100 || Int32.Parse(CS) > 100 || Int32.Parse(FS) > 100 then
             MessageBox.Show("Max value in grade is 100%") |> ignore
         elif  Int32.Parse(English) < 0 || Int32.Parse(CS) < 0 || Int32.Parse(FS) < 0 then
             MessageBox.Show("Min value in grade is 0%") |> ignore
         else
             let englishVal = Double.Parse(English)
             let csVal = Double.Parse(CS)
             let fsVal = Double.Parse(FS)
             let grades = ((englishVal + csVal + fsVal) / 300.0) * 100.0

             use connection = new MySqlConnection(connectionString)
             connection.Open()

             let query = "INSERT INTO studen_info (name, ID, grades , English , CS ,FS) VALUES (@name, @ID, @grades , @English ,@CS ,@FS)"
             use command = new MySqlCommand(query, connection)
             command.Parameters.AddWithValue("@name", name) |> ignore
             command.Parameters.AddWithValue("@ID", Int32.Parse(ID)) |> ignore
             command.Parameters.AddWithValue("@grades", grades) |> ignore
             command.Parameters.AddWithValue("@English", Int32.Parse(English)) |> ignore
             command.Parameters.AddWithValue("@CS", Int32.Parse(CS)) |> ignore
             command.Parameters.AddWithValue("@FS", Int32.Parse(FS)) |> ignore

             let rowsAffected = command.ExecuteNonQuery()
             if rowsAffected > 0 then
                 MessageBox.Show("Data added successfully!") |> ignore
             else
                 MessageBox.Show("Failed to add data.") |> ignore
     with
     | ex -> MessageBox.Show($"Error: {ex.Message}") |> ignore
 )

// زر "Edit"
let editButton = new Button(Text = "Edit", Top = 460, Left = 140, Width = 100)
editButton.Click.Add(fun _ ->
    try
        let name = nameTextBox.Text
        let ID = IDTextBox.Text
        let English = EnglishTextBox.Text
        let CS = CSTextBox.Text
        let FS = FSTextBox.Text
        let searchValue = searchTextBox.Text

        if String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(ID) then
            MessageBox.Show("Please fill all fields to update.") |> ignore
        elif  Int32.Parse(English) > 100 || Int32.Parse(CS) > 100 || Int32.Parse(FS) > 100 then
            MessageBox.Show("Max value in grade is 100%") |> ignore
        elif  Int32.Parse(English) < 0 || Int32.Parse(CS) < 0 || Int32.Parse(FS) < 0 then
            MessageBox.Show("Min value in grade is 0%") |> ignore
        else
            let englishVal = Double.Parse(English)
            let csVal = Double.Parse(CS)
            let fsVal = Double.Parse(FS)
            let grades = ((englishVal + csVal + fsVal) / 300.0) * 100.0
           
            use connection = new MySqlConnection(connectionString)
            connection.Open()

            let query = "UPDATE studen_info SET name = @name, grades = @grades, English = @English, CS = @CS, FS = @FS WHERE ID = @ID"
            use command = new MySqlCommand(query, connection)
            command.Parameters.AddWithValue("@name", name) |> ignore
            command.Parameters.AddWithValue("@ID", Int32.Parse(ID)) |> ignore
            command.Parameters.AddWithValue("@grades", grades) |> ignore
            command.Parameters.AddWithValue("@English", englishVal) |> ignore
            command.Parameters.AddWithValue("@CS", csVal) |> ignore
            command.Parameters.AddWithValue("@FS", fsVal) |> ignore

            let rowsAffected = command.ExecuteNonQuery()
            if rowsAffected > 0 then
                MessageBox.Show("Data updated successfully!") |> ignore
            else
                MessageBox.Show("Failed to update data.") |> ignore
    with
    | ex -> MessageBox.Show($"Error: {ex.Message}") |> ignore
)



// زر "Delete"
let deleteButton = new Button(Text = "Delete", Top = 460, Left = 20, Width = 100)
deleteButton.Click.Add(fun _ ->
    try
        let searchValue = searchTextBox.Text

        if String.IsNullOrWhiteSpace(searchValue) then
            MessageBox.Show("Please enter a ID to delete.") |> ignore
        else
            use connection = new MySqlConnection(connectionString)
            connection.Open()

            let query = "DELETE FROM studen_info WHERE ID = @ID"
            use command = new MySqlCommand(query, connection)
            command.Parameters.AddWithValue("@ID", Int32.Parse(searchValue)) |> ignore

            let rowsAffected = command.ExecuteNonQuery()
            if rowsAffected > 0 then
                MessageBox.Show("Data deleted successfully!") |> ignore
            else
                MessageBox.Show("Failed to delete data.") |> ignore
    with
    | ex -> MessageBox.Show($"Error: {ex.Message}") |> ignore
)



// زر "Clear"
    let clearButton = new Button(Text = "Clear", Top = 460, Left = 300, Width = 100)
    clearButton.Click.Add(fun _ ->
        nameTextBox.Text <- ""
        IDTextBox.Text <- ""
        gradesTextBox.Text <- ""
        EnglishTextBox.Text <- ""
        CSTextBox.Text <- ""
        FSTextBox.Text <- ""
        searchTextBox.Text <- ""
    )

    // إضافة كل العناصر للنافذة
form.Controls.AddRange(
    [|refreshButton ; studentGridView ; nameLabel; nameTextBox;
       IDLabel; IDTextBox;
        gradesTextBox;FSTextBox;FSLabel;CSTextBox;CSLabel;EnglishTextBox;EnglishLabel;
       searchLabel; searchTextBox; searchButton;
       addButton; editButton; deleteButton; clearButton  |]
)

backButton.Click.Add(fun _ ->
    form.Hide()
    mainForm.Show()
)

form.Controls.Add(backButton :> Control) // إضافة زر العودة
form
////////////////////////////////////





///////////////////////////////////


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




let showIdPrompt () =
    let idForm = new Form(Text = "Enter Student ID", Width = 300, Height = 150, StartPosition = FormStartPosition.CenterParent)
    let idLabel = new Label(Text = "Student ID:", Top = 20, Left = 20, AutoSize = true)
    let idTextBox = new TextBox(Top = 20, Left = 120, Width = 150)
    let okButton = new Button(Text = "OK", Top = 60, Left = 60, Width = 80)
    let cancelButton = new Button(Text = "Cancel", Top = 60, Left = 160, Width = 80)
    let mutable result: string option = None
    
    okButton.Click.Add(fun _ -> 
        result <- Some idTextBox.Text
        idForm.Close()  // Close the form after clicking OK
    )
    cancelButton.Click.Add(fun _ -> 
        result <- None;  // Set result to None if Cancel is clicked
        idForm.Close()  // Close the form when Cancel is clicked
    )

    idForm.Controls.AddRange([| idLabel :> Control; idTextBox :> Control; okButton :> Control; cancelButton :> Control |])
    idForm.ShowDialog() |> ignore  // Show the form and wait for user interaction
    result