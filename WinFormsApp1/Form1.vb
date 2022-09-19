Imports System.Windows
Imports System.Windows.Forms.DataFormats
Imports Microsoft.Office.Interop
Imports Microsoft.Office.Interop.Excel

Public Class Form1
    Dim array(,) As Object
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim depth As Integer = Integer.Parse(TextBox2.Text) 'глубина
        Dim numSubscribers As Integer = Integer.Parse(TextBox3.Text) 'количество абонентов
        Dim answerVariants As String() = TextBox1.Lines 'варианты
        Dim answerVariantsLength As Integer = answerVariants.Length 'количество вариантов
        Dim answers(numSubscribers, depth) As Integer 'массив со всеми возможными вариантами ответов
        Dim answersStr(numSubscribers) As String 'массив со всеми возможными вариантами ответов
        Dim answersResult(answerVariantsLength ^ depth) As Integer
        Dim tableNum(numSubscribers, depth) As Integer 'переведенный в числовой формат входной массив
        Dim tableNumStr(numSubscribers) As String 'переведенный в числовой формат входной массив
        Dim cell As String
        Dim oneRow(depth) As Integer
        Dim Row As ListViewItem
        Dim Row2 As ListViewItem
        Dim Row3 As ListViewItem

        With ListView1
            .Clear()
            .View = View.Details
            .GridLines = True
            .MultiSelect = False
            For i As Integer = 0 To (depth)
                .Columns.Add(CStr(i), 50, HorizontalAlignment.Left)
            Next
            .FullRowSelect = True
        End With

        With ListView2
            .Clear()
            .View = View.Details
            .GridLines = True
            For i As Integer = 0 To (depth - 1)
                .Columns.Add(CStr(i), 50, HorizontalAlignment.Left)
            Next
            .FullRowSelect = True
        End With

        With ListView3
            .Clear()
            .View = View.Details
            .GridLines = True
            For i As Integer = 0 To (depth - 1)
                .Columns.Add(CStr(i), 50, HorizontalAlignment.Left)
            Next
            .FullRowSelect = True
        End With

        For i As Integer = 0 To (numSubscribers - 1) 'перевод входного файла в массив с цифрами
            System.Windows.Forms.Application.DoEvents()
            If (((i + 1) Mod 100) = 0) Then
                Label3.Text = "Обработано строк из входного файла: " + CStr(i + 1) + " из " + CStr(numSubscribers)
            End If
            For j As Integer = 0 To (depth - 1)
                cell = array(i + 1, j + 1)
                Dim tim As Boolean = False
                For k As Integer = 0 To (answerVariants.Length - 1)
                    If (answerVariants(k) = cell) Then
                        tableNum(i, j) = k + 1
                        tableNumStr(i) = tableNumStr(i) + CStr(k + 1) + "/"
                        tim = True
                    End If
                Next
                If (tim = False) Then
                    tableNum(i, j) = 0
                    tableNumStr(i) = tableNumStr(i) + CStr(0) + "/"
                End If
            Next
            'Row3 = ListView3.Items.Add(tableNum(i, 0))
            'For z As Integer = 1 To depth
            '    Row3.SubItems.Add(tableNum(i, z))
            'Next
        Next

        Label3.Text = "Творится магия. 1 из 3"
        System.Windows.Forms.Application.DoEvents()
        Dim Col As New List(Of String) 'заполнение массива answers всеми вариантами ответов существующими в файле
        For Each I As String In tableNumStr
            If Not Col.Contains(I) Then
                Col.Add(I)
            End If
        Next
        answersStr = Col.ToArray

        Label3.Text = "Творится магия. 2 из 3"
        System.Windows.Forms.Application.DoEvents()
        For i As Integer = 0 To (answersStr.Length - 1) 'сравнение и заполнение массива количеством совпадений
            For j As Integer = 1 To (numSubscribers)
                If answersStr(i) = tableNumStr(j) Then
                    answersResult(i) = answersResult(i) + 1
                End If
            Next
        Next

        Label3.Text = "Творится магия. 3 из 3"
        System.Windows.Forms.Application.DoEvents()
        Dim ans1() As String 'перевод массива с ответами из типа string(одномерный) в тип integer(двумерный)
        Dim sep() As Char = {"/"}
        For j = 0 To (answersStr.Length - 1)
            If answersStr(j) <> Nothing Then
                ans1 = answersStr(j).Split(sep)
                For i As Integer = 0 To (depth - 1)
                    answers(j, i) = CInt(ans1(i))
                Next
            End If
        Next

        For i As Integer = 0 To (answersStr.Length - 1) 'вывод результатов
            If answers(i, 0) = 0 Then
                Row = ListView1.Items.Add("!Нет в вариантах")
            Else
                Row = ListView1.Items.Add(answerVariants(answers(i, 0) - 1))
            End If
            For j As Integer = 1 To (depth - 1)
                If answers(i, j) = 0 Then
                    Row.SubItems.Add("!Нет в вариантах")
                Else
                    Row.SubItems.Add(answerVariants(answers(i, j) - 1))
                End If
            Next
            Row.SubItems.Add(answersResult(i))
            If (((i + 1) Mod 10) = 0) Then
                System.Windows.Forms.Application.DoEvents()
                Label3.Text = "Вывод результатов. Строка " + CStr(i + 1) + " из " + CStr(answersStr.Length)
                ListView1.Items(i).Selected = True
                ListView1.Items.Item(i).EnsureVisible()
                ListView1.Select()
            End If
        Next
        ListView1.MultiSelect = True
        Label3.Text = "Готово"
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Call ShowDialog.
        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        ' Test result.
        If (result = DialogResult.OK) Then
            Label3.Text = "Полный путь: " + OpenFileDialog1.FileName

            ' Create new Application.
            Dim excel As Application = New Application

            ' Open Excel spreadsheet.
            Dim w As Workbook = excel.Workbooks.Open(OpenFileDialog1.FileName)

            ' Loop over all sheets.
            For i As Integer = 1 To w.Sheets.Count

                ' Get sheet.
                Dim sheet As Worksheet = w.Sheets(i)

                ' Get range.
                Dim r As Range = sheet.UsedRange

                ' Load all cells into 2d array.
                array = r.Value(XlRangeValueDataType.xlRangeValueDefault)

                ' Scan the cells.
                If Array IsNot Nothing Then
                    Console.WriteLine("Length: {0}", array.Length)
                    'MsgBox(array.Length)

                    ' Get bounds of the array.
                    Dim bound0 As Integer = Array.GetUpperBound(0)
                    Dim bound1 As Integer = Array.GetUpperBound(1)

                    Console.WriteLine("Dimension 0: {0}", bound0)
                    Console.WriteLine("Dimension 1: {0}", bound1)

                    ' Loop over all elements.
                    For j As Integer = 1 To bound0
                        For x As Integer = 1 To bound1
                            Dim s1 As String = Array(j, x)
                            Console.Write(s1)
                            Console.Write(" "c)
                            If ((j Mod 100) = 0) Then
                                Label3.Text = "Открывается файл. Считано строк: " + CStr(j) + " из " + CStr(bound0)
                                System.Windows.Forms.Application.DoEvents()
                            End If
                        Next
                        Console.WriteLine()
                    Next
                    Label3.Text = "Открывается файл. Считано строк: " + CStr(bound0) + " из " + CStr(bound0)
                    TextBox3.Text = CStr(bound0)
                End If
            Next

            ' Close.
            w.Close()
            excel.Quit()
            ' Do something.
        End If
    End Sub

    Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles Button3.Click
        Dim SFD As New SaveFileDialog With {.Filter = "Файлы excel|*.csv", .FileName = "Результат " + Now.ToLongDateString}
        If SFD.ShowDialog = DialogResult.OK Then
            If System.IO.File.Exists(SFD.FileName) = True Then
                System.IO.File.Delete(SFD.FileName)
            End If
            ExportListview2Excel(ListView1, SFD.FileName)
        End If
    End Sub

    Function ExportListview2Excel(ByVal lstview As ListView, put As String) As Boolean

        Dim csvFileContents As New System.Text.StringBuilder

        Dim CurrLine As String = String.Empty

        'Write out the column names as headers for the csv file.

        For columnIndex As Int32 = 0 To lstview.Columns.Count - 1

            CurrLine &= (String.Format("{0};", lstview.Columns(columnIndex).Text))

        Next

        'Remove trailing comma

        csvFileContents.AppendLine(CurrLine.Substring(0, CurrLine.Length - 1))

        CurrLine = String.Empty

        'Write out the data.

        For Each item As ListViewItem In lstview.Items

            For Each subItem As ListViewItem.ListViewSubItem In item.SubItems

                CurrLine &= (String.Format("{0};", subItem.Text))

            Next

            'Remove trailing comma

            csvFileContents.AppendLine(CurrLine.Substring(0, CurrLine.Length - 1))

            CurrLine = String.Empty

        Next

        'Create the file.

        Dim Sys As New System.IO.StreamWriter(put, 32, System.Text.Encoding.UTF8)

        Sys.WriteLine(csvFileContents.ToString)

        Sys.Flush()

        Sys.Dispose()

        Return (0)
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ListView2.Visible = False
        ListView3.Visible = False
        Label3.Text = ""
    End Sub

End Class
