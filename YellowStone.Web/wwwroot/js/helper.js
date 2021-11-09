function Notify(message, msgtype = "danger") {
    $.notify({
        title: "",
        message: message,
    }, {
            type: 'pastel-' + msgtype,  //info or danger
            delay: 5000,
            template: '<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0}" role="alert">' +
                '<span data-notify="title">{1}</span>' +
                '<span data-notify="message">{2}</span>' +
                '</div>'
        });
}

function Dialog(message, action) {
    $.confirm({
        title: '',
        content: message,
        buttons: {
            ok: {
                text: "ok",
                btnClass: 'btn-primary',
                keys: ['enter'],
                action: action
            },
            cancel: function () { }
        }
    });
}

function DialogCancel(message, action, cancel) {
    $.confirm({
        title: '',
        content: message,
        buttons: {
            ok: {
                text: "ok",
                btnClass: 'btn-primary',
                keys: ['enter'],
                action: action
            },
            cancel: cancel
        }
    });
}


function DialogOkOnly(message, action) {
    $.confirm({
        title: '',
        content: message,
        buttons: {
            ok: {
                text: "ok",
                btnClass: 'btn-primary',
                keys: ['enter'],
                action: action
            }
        }
    });
}


function DialogInput(message, action) {


    $.confirm({
        title: 'Reject Onboarding Request',
        content: '' +
            '<form action="" class="formName">' +
            '<div class="form-group">' +
            '<label>' + message + '</label>' +
            '<textarea rows = "3" cols = "35" name = "description">' +
            '</textarea>' +
            '</div>' +
            '</form>',
        buttons: {
            formSubmit: {
                text: 'Submit',
                btnClass: 'btn-blue',
                action: function () {
                    var comment = this.$content.find('.name').val();
                    if (!comment) {
                        $.alert('Please enter a comment');
                        return false;
                    }
                    action(comment);
                  //  $.alert('Your name is ' + comment);
                }
            },
            cancel: function () {
                //close
            },
        },
        onContentReady: function () {
            // bind to events
            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });


}


function AjaxRequest(url, type, data, timeoutUrl, callback, errorCallBack) {
    $.ajax({
        cache: false,
        url: url,
        data: data,
        type: type,
        beforeSend: function () {
            document.getElementById("overlay").style.display = 'block';
        }
    }).done(function (response) {
        document.getElementById("overlay").style.display = 'none';
        callback(response);
    })
        .fail(function (jqxhr, status, error) {
            if (error == 'Unauthorized') {
                window.location.href = timeoutUrl;
            } else {
                if (errorCallBack) {
                    errorCallBack(jqxhr.responseText);
                } else {
                    $.alert(jqxhr.responseText);
                }
            }
            document.getElementById("overlay").style.display = 'none';
        });
}

function AjaxRequestNoLoaders(url, type, data, timeoutUrl, callback, errorCallBack) {
    $.ajax({
        cache: false,
        url: url,
        data: data,
        type: type,
        beforeSend: function () {
        }
    }).done(function (response) {
        callback(response);
    })
        .fail(function (jqxhr, status, error) {
            if (error == 'Unauthorized') {
                window.location.href = timeoutUrl;
            } else {
                if (errorCallBack) {
                    errorCallBack(jqxhr.responseText);
                } else {
                    $.alert(jqxhr.responseText);
                }
            }
        });
}

