$(function(){

    //初始化编辑器
    InitEditor();

    //获取博客列表
    GetBlogs();

    //添加标签
    $('#addTag').on('click',function(){
        var tagName= $('#tagName').val();
        $.post('@Url.Action("AddTag","Home")',{tagName:tagName},function(){
            location.reload();
        })
    });

    //添加类别
    $('#addCategory').on('click',function(){
        var categoryName = $('#categoryName').val();
        $.post('@Url.Action("AddCategory","Home")',{categoryName:categoryName},function(){
            location.reload();
        })
    });

    //添加类别
    $('#addBlog').on('click',function(){
        
        var title = $('#title').val();

        var content = $('#content').val();

        var categoryId =  $('#categoryselect').find('option:selected').attr('data-categoryid');

        var tagId = $('#tagselect').find('option:selected').attr('data-tagid');

        $.post('@Url.Action("AddBlog","Home")', { title:title, content:content, categoryId:categoryId,tagId:tagId }, function(responseText) {
                location.reload();
        });            
    })
})


function GetBlogs(){
    $.get('@Url.Action("GetBlogs","Home")',
    {
        pageIndex:1,
        pageSize:10
    },function(responseText){      
        var tableTemplate = Handlebars.compile($('#blogs-template').html());
        $('#blogs').html(tableTemplate(responseText));
    })
}

//删除标签
function removeTag(tagId){
    $.post('@Url.Action("RemoveTag","Home")',{tagId:tagId},function(responseText){
        location.reload();
    })
}

//删除类别
function removeCategory(categoryId){
    $.post('@Url.Action("RemoveCategory","Home")',{categoryId:categoryId},function(responseText){
        location.reload();
    })
}

//删除文章
function removeBlog(blogId){
    
}

function InitEditor() {
    
    var editor = new wangEditor('content');

    // 普通的自定义菜单
    editor.config.menus = [
        'source',
        '|',
        'bold',
        'underline',
        'italic',
        'eraser',
        'forecolor',
        '|',
        'fontfamily',
        'fontsize',
        '|',
        'emotion',
        '|',
        'img',
        'insertcode',
        '|',
        'undo',
        'redo'
    ];

    editor.config.uploadImgUrl = '@Url.Action("UploadImage","Home",new{area=""})';

    editor.config.hideLinkImg = true;

    editor.create();

    window.editor = editor;
}
