using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewBlogger.Dto;

namespace NewBlogger.Application.Interface
{
    public interface ITagService
    {

        IList<TagDto> GetTags();

        Task AddTagAsync(String tagName);

        Task RemoveTagAsync(Guid tagId);
    }
}
