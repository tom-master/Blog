﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewBlogger.Application.Interface;
using NewBlogger.Dto;
using NewBlogger.Model;
using NewBlogger.Repository;

namespace NewBlogger.Application
{
    public class TagService : ITagService
    {
        private readonly IRepository<NewBlogger.Model.Tag> _tagRepository;

        public TagService(IRepository<Model.Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public IList<TagDto> GetTags()
        {
            return _tagRepository.Find().Select(s => new TagDto
            {
                AddTime = s.AddTime,
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }

        public async Task AddTagAsync(String tagName) => await _tagRepository.AddAsync(new Tag(tagName));


        public async Task RemoveTagAsync(Guid tagId) => await _tagRepository.RemoveAsync(tagId);

    }
}