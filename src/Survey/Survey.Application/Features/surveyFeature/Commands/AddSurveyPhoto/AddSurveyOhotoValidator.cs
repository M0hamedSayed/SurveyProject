using FluentValidation;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;

namespace Survey.Application.Features.surveyFeature.Commands.AddSurveyPhoto
{
    public class AddSurveyOhotoValidator : AbstractValidator<AddSurveyPhotoCommand>
    {
        public AddSurveyOhotoValidator() 
        {
            ImageUploadValidator();
        }

        public void ImageUploadValidator()
        {
            RuleFor(c => c.Image)
                .NotNull().WithMessage("File is required.")
                .Must(BeAValidImage).WithMessage("Only image files (JPEG, PNG) are allowed.")
                .Must(BeWithinSizeLimit).WithMessage("Image size cannot exceed 10MB.")
                .Must(HaveValidResolution).WithMessage("Image resolution must not exceed 1200x1200 pixels.");
        }

        private bool BeAValidImage(IFormFile? file)
        {
            if (file == null || file.Length == 0) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName)?.ToLower();
            return allowedExtensions.Contains(extension);
        }
        private bool BeWithinSizeLimit(IFormFile? file)
        {
            if (file == null) return true;
            return file.Length <= 10 * 1024 * 1024; // 10MB
        }
        private bool HaveValidResolution(IFormFile file)
        {
            if (file == null) return false;

            using var stream = file.OpenReadStream();
            if (BeAValidImage(file))
            {
                using var image = Image.Load(stream);  // ✅ Correct method from SixLabors.ImageSharp
                return image.Width <= 1200 && image.Height <= 1200;
            }
            return false;
            
        }
    }
}
