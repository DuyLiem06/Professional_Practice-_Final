using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Professional_Practice
{
    public partial class SmartStart_Check_in_Manager : Form
    {
        public SmartStart_Check_in_Manager()
        {
            InitializeComponent();
        }

        // 1. SỰ KIỆN KHI FORM VỪA MỞ LÊN (LOAD)
        private void SmartStart_Check_in_Manager_Load(object sender, EventArgs e)
        {
            // Thiết lập tên cột
            dgvDanhSach.ColumnCount = 3;
            dgvDanhSach.Columns[0].Name = "Full Name";
            dgvDanhSach.Columns[1].Name = "Phone Number";
            dgvDanhSach.Columns[2].Name = "School Name";

            // Tự động dãn cột cho đẹp
            dgvDanhSach.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // QUAN TRỌNG: Chế độ chọn cả dòng (Giúp nút Xóa hoạt động chính xác)
            dgvDanhSach.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDanhSach.MultiSelect = false; // Chỉ cho phép xóa từng người một để tránh lỗi

            // Gán sự kiện cho các nút bằng Code (Đảm bảo nút luôn hoạt động đúng hàm)
            // Lưu ý: Cần xóa các sự kiện cũ bên giao diện thiết kế nếu bị trùng
            btnAdd.Click -= btnAdd_Click; // Xóa trước để tránh bị gán 2 lần
            btnAdd.Click += new EventHandler(btnAdd_Click);

            btnSave.Click -= btnSave_Click;
            btnSave.Click += new EventHandler(btnSave_Click);

            btnDelete.Click -= btnDelete_Click;
            btnDelete.Click += new EventHandler(btnDelete_Click);

            search.TextChanged -= search_TextChanged;
            search.TextChanged += new EventHandler(search_TextChanged);
        }

        // 2. NÚT ADD CHECK-IN (THÊM)
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Please enter Full Name and Phone Number!", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvDanhSach.Rows.Add(txtHoTen.Text, txtPhone.Text, txtSchool.Text);

            txtHoTen.Clear();
            txtPhone.Clear();
            txtSchool.Clear();
            txtHoTen.Focus();
        }

        // 3. NÚT DELETE ROW (XÓA) - Đã sửa lỗi chọn dòng
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có dòng nào được chọn không
            if (dgvDanhSach.SelectedRows.Count > 0)
            {
                // Lấy dòng đang chọn
                DataGridViewRow selectedRow = dgvDanhSach.SelectedRows[0];

                // Kiểm tra xem có phải dòng trắng cuối cùng không (IsNewRow)
                if (selectedRow.IsNewRow)
                {
                    MessageBox.Show("Cannot delete the empty new row!", "Warning");
                    return;
                }

                // Hỏi xác nhận trước khi xóa
                if (MessageBox.Show("Are you sure you want to delete this person?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dgvDanhSach.Rows.Remove(selectedRow);
                }
            }
            else
            {
                // Trường hợp người dùng click vào một ô (Cell) nhưng chưa chọn cả dòng
                // Đoạn code này hỗ trợ xóa ngay cả khi SelectionMode không phải FullRowSelect
                if (dgvDanhSach.CurrentRow != null && !dgvDanhSach.CurrentRow.IsNewRow)
                {
                    if (MessageBox.Show("Are you sure you want to delete this person?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvDanhSach.Rows.Remove(dgvDanhSach.CurrentRow);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.", "Notice");
                }
            }
        }

        // 4. NÚT EXPORT EXCEL (LƯU FILE CSV)
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvDanhSach.Rows.Count <= 1)
            {
                MessageBox.Show("No data to export!", "Empty List");
                return;
            }

            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.FileName = "SmartStart_List.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine("Full Name,Phone Number,School Name");
                        foreach (DataGridViewRow row in dgvDanhSach.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                string name = row.Cells[0].Value?.ToString() ?? "";
                                string phone = row.Cells[1].Value?.ToString() ?? "";
                                string school = row.Cells[2].Value?.ToString() ?? "";
                                sw.WriteLine($"{name},{phone},{school}");
                            }
                        }
                    }
                    MessageBox.Show("Export Successful!", "Success");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // 5. CHỨC NĂNG TÌM KIẾM
        private void search_TextChanged(object sender, EventArgs e)
        {
            string keyword = search.Text.ToLower();

            foreach (DataGridViewRow row in dgvDanhSach.Rows)
            {
                if (!row.IsNewRow) // Bỏ qua dòng trống cuối cùng
                {
                    // Lấy giá trị tên ở cột đầu tiên
                    string name = row.Cells[0].Value?.ToString().ToLower() ?? "";

                    // Nếu tên chứa từ khóa -> Hiện, ngược lại -> Ẩn
                    if (name.Contains(keyword))
                    {
                        row.Visible = true;
                    }
                    else
                    {
                        // Dùng CurrencyManager để ẩn dòng (tránh lỗi khi đang edit)
                        row.Visible = false;
                    }
                }
            }
        }

        // HÀM RỖNG - ĐỂ TRÁNH LỖI NẾU DESIGNER VẪN GỌI NÓ
        private void label1_Click(object sender, EventArgs e) { }
    }
}
