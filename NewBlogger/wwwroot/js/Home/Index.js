$(function () {
    $('#index').addClass('active');
    getBlog(0);
});

//获取文章列表
function getBlog(current_page) {
    $.get('/Home/GetBlogs',
    {
        pageIndex: current_page === 0 ? 1 : current_page + 1,
        pageSize: 5,
        categoryId: $('#categoryId').val()
    }, function (responseText) {
        var tableTemplate = Handlebars.compile($("#blogs-template").html());

        Handlebars.registerHelper('ReplaceText',function(dateTime){
            return dateTime.replace(/T/,' ');
        })

        $('#blogs').html(tableTemplate(responseText));
        $('#pagination_setting').attr('count', responseText.totalCount);
        initPagination(current_page);
    });
}

//初始化分页
function initPagination(current_page) {
    $('#pagination').pagination(parseInt($('#pagination_setting').attr('count')), {
        current_page: current_page,
        items_per_page: 5,
        num_display_entries: 3,
        num_edge_entries: 1,
        callback: getBlog,
        prev_text: '上一页',
        next_text: '下一页'
    });
}
