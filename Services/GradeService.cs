using MagistriMVC.Models;
using MagistriMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagistriMVC.Services
{
	public class GradeService
	{
		ApplicationDbContext dbContext;
		public GradeService(ApplicationDbContext context) {
			this.dbContext = context;
		}
		public async Task<IEnumerable<Grade>> GetAllAsync()
		{
			return await dbContext.Grades.Include(n=>n.Student).Include(c=>c.Subject).ToListAsync();
		}
		public async Task<GradesDropdownsViewModel> GetGradesDropdownsValues()
		{
			var gradesDropdownsData = new GradesDropdownsViewModel()
			{
				Students = await dbContext.Students.ToListAsync(),
				Subjects = await dbContext.Subjects.ToListAsync()
		};
			return gradesDropdownsData;
		}

        public async Task CreateAsync(GradesViewModel newGrade)
        {
			var gradeToInsert = new Grade()
			{
				Student = dbContext.Students.FirstOrDefault(st => st.Id == newGrade.StudentId),
				Subject = dbContext.Subjects.FirstOrDefault(su => su.Id == newGrade.SubjectId),
				Mark = newGrade.Mark,
				What = newGrade.What,
				Date = DateTime.Today
			};
			if (gradeToInsert.Subject !=null && gradeToInsert.Student != null)
			{
                await dbContext.Grades.AddAsync(gradeToInsert);
                await dbContext.SaveChangesAsync();
            }
        }
		public async Task<Grade> GetByIdAsync(int id)
		{
			return await dbContext.Grades.Include(st => st.Student).Include(su => su.Subject).FirstOrDefaultAsync(g => g.Id == id);
		}

        public async Task UpdateAsync(int id, GradesViewModel updatedGrade)
        {
			var dbGrade = await dbContext.Grades.FirstOrDefaultAsync(gr=>gr.Id == id);
			if (dbGrade != null)
			{
				dbGrade.Student = await dbContext.Students.FirstOrDefaultAsync(st=>st.Id == updatedGrade.StudentId);
                dbGrade.Subject = await dbContext.Subjects.FirstOrDefaultAsync(su => su.Id == updatedGrade.SubjectId);
				dbGrade.What = updatedGrade.What;
				dbGrade.Mark = updatedGrade.Mark;
				dbGrade.Date = updatedGrade.Date;
            }
			dbContext.Update(dbGrade);
			dbContext.SaveChanges();
        }
		public async Task DeleteAsync(int id)
		{
			var gradeToDelete = await dbContext.Grades.FirstOrDefaultAsync(gr => gr.Id == id);
			dbContext.Grades.Remove(gradeToDelete);
			await dbContext.SaveChangesAsync();
		}
    }
}
