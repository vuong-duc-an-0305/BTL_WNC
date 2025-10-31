using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClassManagement.Data;
using OnlineClassManagement.Models;
using System.Threading.Tasks;

namespace OnlineClassManagement.Controllers
{
    public class ClassesController : TeacherBaseController
    {
        private readonly ApplicationDbContext _context;

        public ClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Classes
        public async Task<IActionResult> Index()
        {
            // Lấy TempData (nếu có) từ các action Create, Edit, Delete
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            int currentTeacherId = CurrentTeacherId; 
            var classes = await _context.Classes
                .Where(c => c.TeacherId == currentTeacherId)
                .ToListAsync();
            return View(classes);
        }

        // GET: /Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Nhận thông báo từ Action Edit
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            if (id == null) return NotFound();
            var @class = await _context.Classes
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (@class == null) return NotFound();
            if (@class.TeacherId != CurrentTeacherId) return Forbid();
            return View(@class);
        }

        // GET: /Classes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Classes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ClassName,ClassCode,Description,AcademicYear,Semester,MaxStudents")] Class @class)
        {
            @class.TeacherId = CurrentTeacherId;
            ModelState.Remove(nameof(@class.TeacherId));
            ModelState.Remove(nameof(@class.Teacher));

            if (await _context.Classes.AnyAsync(c => c.ClassCode == @class.ClassCode))
            {
                ModelState.AddModelError("ClassCode", "Mã lớp này đã tồn tại. Vui lòng chọn mã khác.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    @class.CreatedAt = DateTime.Now;
                    @class.UpdatedAt = DateTime.Now; 
                    @class.IsActive = true; 

                    _context.Add(@class);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Tạo lớp học mới thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE KEY constraint"))
                    {
                        ModelState.AddModelError("ClassCode", "Mã lớp này đã tồn tại. Vui lòng chọn mã khác.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu vào CSDL. Vui lòng thử lại.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Đã xảy ra lỗi: {ex.Message}");
                }
            }
            return View(@class);
        }

        // GET: /Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();
            if (@class.TeacherId != CurrentTeacherId) return Forbid();
            return View(@class);
        }

        // POST: /Classes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            [Bind("ClassId,ClassName,ClassCode,Description,AcademicYear,Semester,MaxStudents,IsActive")] Class @classFromForm)
        {
            if (id != @classFromForm.ClassId) return NotFound();

            var @classInDb = await _context.Classes.FindAsync(id);
            if (@classInDb == null) return NotFound();
            if (@classInDb.TeacherId != CurrentTeacherId) return Forbid();

            ModelState.Remove(nameof(Class.TeacherId));
            ModelState.Remove(nameof(Class.Teacher));

            if (await _context.Classes.AnyAsync(c => c.ClassCode == @classFromForm.ClassCode && c.ClassId != id))
            {
                ModelState.AddModelError("ClassCode", "Mã lớp này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    @classInDb.ClassName = @classFromForm.ClassName;
                    @classInDb.ClassCode = @classFromForm.ClassCode;
                    @classInDb.Description = @classFromForm.Description;
                    @classInDb.AcademicYear = @classFromForm.AcademicYear;
                    @classInDb.Semester = @classFromForm.Semester;
                    @classInDb.MaxStudents = @classFromForm.MaxStudents;
                    @classInDb.IsActive = @classFromForm.IsActive;
                    @classInDb.UpdatedAt = DateTime.Now; 

                    _context.Update(@classInDb);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Cập nhật lớp học thành công!";
                    return RedirectToAction(nameof(Details), new { id = @classInDb.ClassId });
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE KEY constraint"))
                    {
                        ModelState.AddModelError("ClassCode", "Mã lớp này đã tồn tại.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật CSDL.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Đã xảy ra lỗi: {ex.Message}");
                }
            }
            return View(@classFromForm);
        }

        // GET: /Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var @class = await _context.Classes
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (@class == null) return NotFound();
            if (@class.TeacherId != CurrentTeacherId) return Forbid();
            return View(@class);
        }

        // POST: /Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();
            if (@class.TeacherId != CurrentTeacherId) return Forbid();

            try
            {
                _context.Classes.Remove(@class);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa lớp học thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa lớp này. Lớp học có thể đang chứa dữ liệu (bài tập, sinh viên...).";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Classes/StudentList/5
        public async Task<IActionResult> StudentList(int id)
        {
            int classId = id;
            
            // 1. Kiểm tra quyền sở hữu
            if (!await CheckClassOwnershipAsync(classId))
            {
                return Forbid(); // Lỗi 403
            }

            var @class = await _context.Classes.FindAsync(classId);
            if (@class == null) return NotFound();

            // 2. Lấy danh sách học viên
            var students = await _context.ClassEnrollments
                .Where(e => e.ClassId == classId)
                .Include(e => e.Student) // Tải thông tin User (Student)
                .Select(e => e.Student)
                .Where(s => s.Role == Models.Enums.UserRole.Student) // Đảm bảo là Student
                .ToListAsync();

            // 3. Gửi thông tin sang View
            ViewBag.ClassId = classId;
            ViewBag.ClassName = @class.ClassName;

            return View(students);
        }

        // === HÀM HELPER KIỂM TRA QUYỀN SỞ HỮU ===
        private async Task<bool> CheckClassOwnershipAsync(int classId)
        {
            var @class = await _context.Classes
                .AsNoTracking() // Không cần theo dõi entity
                .FirstOrDefaultAsync(c => c.ClassId == classId);
                
            if (@class == null || @class.TeacherId != CurrentTeacherId)
            {
                return false;
            }
            return true;
        }
    }
}