open System
open System.Windows.Forms
open System.Data
open MySql.Data.MySqlClient

let connectionString = "Server=localhost;Database=student;User Id=root;Password=;"



// نافذة إدخال كلمة المرور
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
    let form = new Form(Text = "Admin Panel", Width = 620, Height = 800)
    let backButton: Button = new Button(Text = "Back to Main", Top = 700, Left = 400, Width = 120)

    let displayStatistics () =
        try
            use connection = new MySqlConnection(connectionString)
            connection.Open()

             // استعلامات الإحصائيات
            let executeScalar query =
                use command = new MySqlCommand(query, connection)
                let result = command.ExecuteScalar()
                if result :? DBNull then "0" else result.ToString()

            let totalStudents = executeScalar "SELECT COUNT(*) FROM studen_info"
            let maxGradeen = executeScalar "SELECT MAX(English) FROM studen_info"
            let maxGradecs = executeScalar "SELECT MAX(CS) FROM studen_info"
            let maxGradefs = executeScalar "SELECT MAX(FS) FROM studen_info"
            let minGradeen = executeScalar "SELECT MIN(English) FROM studen_info"
            let minGradecs = executeScalar "SELECT MIN(CS) FROM studen_info"
            let minGradefs = executeScalar "SELECT MIN(FS) FROM studen_info"
            let failedStudentsCounten = executeScalar "SELECT COUNT(*) FROM studen_info WHERE English < 50"
            let failedStudentsCountcs = executeScalar "SELECT COUNT(*) FROM studen_info WHERE CS < 50"
            let failedStudentsCountfs = executeScalar "SELECT COUNT(*) FROM studen_info WHERE FS < 50"
            let topStudentsCounten = executeScalar "SELECT COUNT(*) FROM studen_info WHERE English >= 50"
            let topStudentsCountcs = executeScalar "SELECT COUNT(*) FROM studen_info WHERE CS >= 50"
            let topStudentsCountfs = executeScalar "SELECT COUNT(*) FROM studen_info WHERE FS >= 50"
             // حساب المتوسطات
            let calculateAverage query =
                let result = executeScalar query
                if result = "0" then "0%" else String.Format("{0:F2}%", Convert.ToDouble(result))
            let averagePercentageen = calculateAverage "SELECT (COUNT(*) * 100.0 / (SELECT COUNT(*) FROM studen_info)) FROM studen_info WHERE English >= 50"
            let averagePercentagecs = calculateAverage "SELECT (COUNT(*) * 100.0 / (SELECT COUNT(*) FROM studen_info)) FROM studen_info WHERE CS >= 50"
            let averagePercentagefs = calculateAverage "SELECT (COUNT(*) * 100.0 / (SELECT COUNT(*) FROM studen_info)) FROM studen_info WHERE FS >= 50"

                 // تحديث واجهة الإحصائيات
            let labels = [
                 "Total Students: " + totalStudents, 510, 20
                 "Max Grade in English: " + maxGradeen, 540, 20
                 "Max Grade in CS: " + maxGradecs, 540, 220
                 "Max Grade in FS: " + maxGradefs, 540, 420
                 "Min Grade in English: " + minGradeen, 570, 20
                 "Min Grade in CS: " + minGradecs, 570, 220
                 "Min Grade in FS: " + minGradefs, 570, 420
                 "Failed (<50) in English: " + failedStudentsCounten, 600, 20
                 "Failed (<50) in CS: " + failedStudentsCountcs, 600, 220
                 "Failed (<50) in FS: " + failedStudentsCountfs, 600, 420
                 "Passed (>=50) in English: " + topStudentsCounten, 630, 20
                 "Passed (>=50) in CS: " + topStudentsCountcs, 630, 220
                 "Passed (>=50) in FS: " + topStudentsCountfs, 630, 420
                 "Average in English: " + averagePercentageen, 660, 20
                 "Average in CS: " + averagePercentagecs, 660, 220
                 "Average in FS: " + averagePercentagefs, 660, 420
            ]

            // إزالة التصنيفات القديمة وإضافة الجديدة
            form.Controls
            |> Seq.cast<Control>
            |> Seq.filter (fun control -> control :? Label && control.Top >= 510)
            |> Seq.toList
            |> List.iter (fun control -> form.Controls.Remove(control))
       
            labels
            |> List.iter (fun (text, top, left) ->
                form.Controls.Add(new Label(Text = text, AutoSize = true, Top = top, Left = left))
            )
        with
        | ex -> MessageBox.Show("Error loading statistics: " + ex.Message) |> ignore

    // تحميل الإحصائيات عند فتح النافذة
    displayStatistics()


     // إنشاء جدول لعرض البيانات
    let studentGridView = new DataGridView(Top = 20, Left = 20, Width = 550, Height = 200 ,ReadOnly = true )
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


    refreshButton.Click.Add(fun _ -> 
     loadData()
     displayStatistics() // استدعاء وظيفة عرض الإحصائيات
        
    ) 

   
    // إنشاء الحقول
    let nameLabel = new Label(Text = "Name:", AutoSize = true, Top =300, Left = 20)
    let nameTextBox = new TextBox(Width = 200, Top =300, Left = 80)


    let IDLabel = new Label(Text = "ID:", AutoSize = true, Top =260, Left = 20)
    let IDTextBox = new TextBox(Width = 200, Top =260, Left = 80 )

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
                
                
                let dataTable = new DataTable()
                use adapter = new MySqlDataAdapter(command)
                adapter.Fill(dataTable) |> ignore

                if dataTable.Rows.Count > 0 then
                  studentGridView.DataSource <- dataTable
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

     // زر "Clear"
    let clearButton = new Button(Text = "Clear", Top = 460, Left = 300, Width = 100)
    let clearFields () = 
        [ nameTextBox; IDTextBox; gradesTextBox; EnglishTextBox; CSTextBox; FSTextBox; searchTextBox ]
        |> List.iter (fun box -> box.Text <- "")
    clearButton.Click.Add(fun _ -> clearFields ())


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
                    clearFields()
                    loadData()
                    displayStatistics()
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
            let grades = gradesTextBox.Text
            let english = EnglishTextBox.Text
            let cs = CSTextBox.Text
            let fs = FSTextBox.Text
            let ID = IDTextBox.Text
            let searchValue = searchTextBox.Text
    
            // التحقق من الحقول الفارغة
            if String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(english) ||
               String.IsNullOrWhiteSpace(cs) || String.IsNullOrWhiteSpace(fs) || String.IsNullOrWhiteSpace(ID) then
                MessageBox.Show("Please fill all fields to update.") |> ignore
            elif Int32.Parse(english) > 100 || Int32.Parse(cs) > 100 || Int32.Parse(fs) > 100 then
                MessageBox.Show("Max value in grades is 100%.") |> ignore
            elif Int32.Parse(english) < 0 || Int32.Parse(cs) < 0 || Int32.Parse(fs) < 0 then
                MessageBox.Show("Min value in grades is 0%.") |> ignore
            else
                use connection = new MySqlConnection(connectionString)
                connection.Open()
    
                let queryCheckID = "SELECT ID FROM studen_info WHERE ID = @ID"
                use commandCheckID = new MySqlCommand(queryCheckID, connection)
                commandCheckID.Parameters.AddWithValue("@ID", Int32.Parse(searchValue)) |> ignore
                use readerCheck = commandCheckID.ExecuteReader()
    
                if readerCheck.Read() && readerCheck.GetInt32(0) <> Int32.Parse(ID) then
                    MessageBox.Show("Cannot edit the ID.") |> ignore
                else
                    readerCheck.Close()
    
                    // تحديث الحقول الجديدة في الاستعلام
                    let updateQuery = 
                        "UPDATE studen_info SET name = @name, grades = @grades, English = @English, CS = @CS, FS = @FS WHERE ID = @ID"
                    use updateCommand = new MySqlCommand(updateQuery, connection)
                    updateCommand.Parameters.AddWithValue("@name", name) |> ignore
                    updateCommand.Parameters.AddWithValue("@grades", grades) |> ignore
                    updateCommand.Parameters.AddWithValue("@English", english) |> ignore
                    updateCommand.Parameters.AddWithValue("@CS", cs) |> ignore
                    updateCommand.Parameters.AddWithValue("@FS", fs) |> ignore
                    updateCommand.Parameters.AddWithValue("@ID", Int32.Parse(searchValue)) |> ignore
    
                    let rowsAffected = updateCommand.ExecuteNonQuery()
                    if rowsAffected > 0 then
                        MessageBox.Show("Data updated successfully!") |> ignore
                        clearFields()
                        loadData()
                        displayStatistics()
                    else
                        MessageBox.Show("Failed to update data.") |> ignore
        with
        | ex -> MessageBox.Show($"Error: {ex.Message}") |> ignore)




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
                    clearFields()
                    loadData()
                    displayStatistics()
                else
                    MessageBox.Show("Failed to delete data.") |> ignore
        with
        | ex -> MessageBox.Show($"Error: {ex.Message}") |> ignore
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
let createStudentForm (mainForm: Form) =
    let form = new Form(Text = "Student Panel", Width = 800, Height = 600)
    let backButton = new Button(Text = "Back to Main", Top = 400, Left = 400, Width = 120)

    let IDLabel = new Label(Text = "ID:", AutoSize = true, Top = 20, Left = 20)
    let IDTextBox = new TextBox(Width = 200, Top = 20, Left = 100, ReadOnly = true)

    let nameLabel = new Label(Text = "Name:", AutoSize = true, Top = 100, Left = 20)
    let nameTextBox = new TextBox(Width = 200, Top = 100, Left = 100, ReadOnly = true)

    let gradesLabel = new Label(Text = "Average:", AutoSize = true, Top = 60, Left = 20)
    let gradesTextBox = new TextBox(Width = 200, Top = 60, Left = 100, ReadOnly = true)

    let EnglishLabel = new Label(Text = "English:", AutoSize = true, Top = 20, Left = 320)
    let EnglishTextBox = new TextBox(Width = 150, Top = 20, Left = 380, ReadOnly = true)

    let resultTextBoxen = new TextBox(Width = 50, Top = 20, Left = 530, ReadOnly = true)

    let CSLabel = new Label(Text = "CS:", AutoSize = true, Top = 60, Left = 320)
    let CSTextBox = new TextBox(Width = 150, Top = 60, Left = 380, ReadOnly = true)

    let resultTextBoxcs = new TextBox(Width = 50, Top = 60, Left = 530, ReadOnly = true)

    let FSLabel = new Label(Text = "FS:", AutoSize = true, Top = 100, Left = 320)
    let FSTextBox = new TextBox(Width = 150, Top = 100, Left = 380, ReadOnly = true)

    let resultTextBoxfs = new TextBox(Width = 50, Top = 100, Left = 530, ReadOnly = true)

    let rec promptForID () =
        match showIdPrompt () with
        | Some id when not (String.IsNullOrWhiteSpace(id)) -> 
            try
                use connection = new MySqlConnection(connectionString)
                connection.Open()

                let query = "SELECT name, grades, English, CS, FS, ID FROM studen_info WHERE ID = @ID"
                use command = new MySqlCommand(query, connection)
                command.Parameters.AddWithValue("@ID", Int32.Parse(id)) |> ignore

                use reader = command.ExecuteReader()

                if reader.Read() then
                    nameTextBox.Text <- reader.GetString(0)
                    gradesTextBox.Text <- reader.GetInt32(1).ToString()
                    EnglishTextBox.Text <- reader.GetInt32(2).ToString()
                    CSTextBox.Text <- reader.GetInt32(3).ToString()
                    FSTextBox.Text <- reader.GetInt32(4).ToString()
                    IDTextBox.Text <- reader.GetInt32(5).ToString()

                    // التحقق من النجاح أو الرسوب لكل مادة
                    let englishScore = reader.GetInt32(2)
                    let csScore = reader.GetInt32(3)
                    let fsScore = reader.GetInt32(4)

                    resultTextBoxen.Text <- if englishScore >= 50 then "Passed" else "Failed"
                    resultTextBoxcs.Text <- if csScore >= 50 then "Passed" else "Failed"
                    resultTextBoxfs.Text <- if fsScore >= 50 then "Passed" else "Failed"
        
               

                else
                    MessageBox.Show("ID not found in system. Please try again.") |> ignore
                    promptForID ()  // Retry ID prompt
            with
            | ex -> 
                MessageBox.Show($"Error: {ex.Message}. Please try again.") |> ignore
                promptForID ()  // Retry ID prompt
        | _ -> 
            MessageBox.Show("No data found") |> ignore

    promptForID ()

    backButton.Click.Add(fun _ ->
        form.Hide()
        mainForm.Show()
    )

    form.Controls.AddRange([| 
        nameLabel :> Control; nameTextBox :> Control;
        IDLabel :> Control; IDTextBox :> Control;
        gradesLabel :> Control; gradesTextBox :> Control; 
        EnglishLabel :> Control; EnglishTextBox :> Control; 
        CSLabel :> Control; CSTextBox :> Control; 
        FSLabel :> Control; FSTextBox :> Control; 
        resultTextBoxen :> Control; 
        backButton :> Control;   resultTextBoxcs:> Control; resultTextBoxfs:> Control;
    |])                        
    form








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
