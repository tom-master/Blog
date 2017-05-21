$(function () {
    InitValidForm();

    InitEditor();
});

//初始化表单验证插件
function InitValidForm() {
    $("#leavereply").Validform({ //指明是哪一表单需要验证,名称需加在form表单上;
        tiptype: 4,
        label: ".label",
        showAllError: true,
        datatype: {
            'name': function (value) {
                if (value.length <= 0) {
                    return false;
                }
                return true;
            },
            'email': function (value) {
                if (value.length <= 0) {
                    return false;
                }

                var szReg = /^([a-zA-Z0-9_\.\-])+\@@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

                if (szReg.test(value)) {
                    return true;
                }

                return false;
            },
            'message': function (value) {
                if (value.length <= 0) {
                    return false;
                }
                return true;
            }
        },
        ajaxPost: true,
        callback: function (data) {

            if (parseInt(data.status) === 1) {
                location.reload(true);
            }
        }
    });
}

//初始化文本编辑器插件
function InitEditor() {
    var editor = new wangEditor('message');

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

    editor.config.uploadImgUrl = '/Home/UploadImage';
    editor.config.hideLinkImg = true;
    editor.create();

    window.editor = editor;
}


function setCommentId(id, nickName) {
    $('#replyId').val(id);
    window.editor.$txt.html('');
    window.editor.$txt.append('<p>回复：<a href="javascript:void(0)">' + nickName + '</a></p>');
}
