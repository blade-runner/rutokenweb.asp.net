function GrdBindReady(handler) {

    var called = false;

    function ready() {
        if (called) return;
        called = true;
        handler();
    }

    if (document.addEventListener) {
        document.addEventListener("DOMContentLoaded", function () {
            document.removeEventListener("DOMContentLoaded", arguments.callee, false);
            ready();
        }, false);
    } else if (document.attachEvent) {
        if (document.documentElement.doScroll && window == window.top) {


            function tryScroll() {
                if (called) return;
                try {
                    document.documentElement.doScroll("left");
                    ready();
                } catch (e) {
                    setTimeout(tryScroll, 0);
                }
            }

            tryScroll();
        }
        document.attachEvent("onreadystatechange", function () {
            if (document.readyState === "complete") {
                document.detachEvent("onreadystatechange", arguments.callee);
                ready();
            }
        });
    }


    if (window.addEventListener)
        window.addEventListener('load', ready, false);
    else if (window.attachEvent)
        window.attachEvent('onload', ready);
}

GrdreadyList = [];

function GrdOnReady(handler) {
    if (!GrdreadyList.length) {
        GrdBindReady(function () {
            for (var i = 0; i < GrdreadyList.length; i++) {
                GrdreadyList[i]();
            }
        });
    }
    GrdreadyList.push(handler);
}

/*               -----------=============== token =============-----------------           */
var $GrdToken = {};
GrdOnReady(function () {
    $GrdToken = new Grd();
});



var Grd = function () {


    this.ver = '1.0';

    // ********** events
    var addEvent = (function (window, document) {
        if (document.addEventListener) {
            return function (elem, type, cb) {
                if (elem == null) return;
                if ((elem && !elem.length) || elem === window) {
                    elem.addEventListener(type, cb, false);
                } else if (elem && elem.length) {
                    var len = elem.length;
                    for (var i = 0; i < len; i++) {
                        addEvent(elem[i], type, cb);
                    }
                }
            };
        } else if (document.attachEvent) {
            return function (elem, type, cb) {
                if (elem == null) return;
                if ((elem && !elem.length) || elem === window) {
                    elem.attachEvent('on' + type, function () { return cb.call(elem, window.event); });
                } else if (elem.length) {
                    var len = elem.length;
                    for (var i = 0; i < len; i++) {
                        addEvent(elem[i], type, cb);
                    }
                }
            };
        }
        return function () {
        };
    })(this, document);


    //**************************** ajax
    function getHttpParamsString(obj) {
        var nocache = "_=" + Math.random() + (+new Date());
        var objstring = '';
        for (var i in obj) {
            if (obj.hasOwnProperty(i)) {
                objstring += i + '=' + encodeURIComponent(obj[i]) + '&';
            }

        }
        return objstring + nocache;
    }



    // регистрация обработчиков ответа
    function registreCallbackFunction(xmlHttpRequest, callback, onerror, callbackArgsArray) {
        return function () {
            if (xmlHttpRequest.readyState == 4) {
                xmlHttpRequest.onreadystatechange = null;
                switchForm(true);
                if (!xmlHttpRequest.status || xmlHttpRequest.status >= 200 && xmlHttpRequest.status < 300
                    || xmlHttpRequest.status == 304) {
                    dispToggle(g.rtwAjaxImg, false);
                    callback.apply(xmlHttpRequest, callbackArgsArray);
                } else if (typeof onerror == "function")
                    onerror.apply(xmlHttpRequest, callbackArgsArray);
                else
                    throw new Error(LOCALIZE(rtwAjaxError));
            }


        };
    }




    // запрос на сервер
    function sendRequest(httpParams, callback, onerror, callbackArgsArray) {
        var xmlHttpRequest = new (window.XMLHttpRequest || ActiveXObject)("Msxml2.XMLHTTP");
        if (!xmlHttpRequest)
            throw new Error("XMLHttpRequest error");
        switchForm(false);
        xmlHttpRequest.onreadystatechange =
            registreCallbackFunction(xmlHttpRequest, callback, onerror, callbackArgsArray);
        try {
            dispToggle(g.rtwAjaxImg, true);
            xmlHttpRequest.open("get", encodeURI(g.settings.mainurl + "?" + getHttpParamsString(httpParams)), true);
            xmlHttpRequest.setRequestHeader('X-Requested-With', 'XhrRutoken');
            xmlHttpRequest.send(null);
        } catch (e) {
            dispToggle(g.rtwAjaxImg, false);
            xmlHttpRequest.onreadystatechange = null;
            if (typeof onerror == "function")
                onerror.apply(xmlHttpRequest, callbackArgsArray);
            else
                throw new Error(LOCALIZE(rtwAjaxError));
        }
    }



    var rvalidchars = /^[\],:{}\s]*$/,
        rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g,
        rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g,
        rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g;

    function parseResponse(data) {

        if (typeof data !== "string" || !data) {
            return { type: 'Error', text: LOCALIZE(rtwAjaxErrorNoAuth) };
        }

        if (window.JSON && window.JSON.parse) {
            return window.JSON.parse(data);
        }
        // Logic borrowed from http://json.org/json2.js
        if (rvalidchars.test(data.replace(rvalidescape, "@")
                .replace(rvalidtokens, "]")
                .replace(rvalidbraces, ""))) {

            return (new Function("return " + data))();

        }
        return { type: 'Error', text: LOCALIZE(rtwAjaxErrorNoAuth) };
    }


    // общий обработчик для ошибок
    function errCallback(text) {
        g.token.rtwLogout();
        switchForm(true);
        g.rtwErrorMessage.innerHTML = text;
        dispToggle(g.rtwErrorMessage, true);
        setTimeout(function () {
            fadeout(g.rtwErrorMessage, 99);
        }, 2000);

    }



    // fadeout
    function fadeout(node, start) {
        if (start >= 10) {
            setTimeout(function () { fadeout(node, start); }, 10);
            node.setAttribute('style', 'opacity:0.' + start);
            start--;
        } else {
            node.setAttribute('style', 'display:none;');
        }

    }




    var err = [];
    err[true] = "";
    err[0] = LOCALIZE(rtwErrNoPlugin);
    err[-1] = LOCALIZE(rtwErrNoToken);
    err[-2] = LOCALIZE(rtwErrTokenNoLoggedIn);
    err[-3] = LOCALIZE(rtwErrPinWrong);
    err[-4] = LOCALIZE(rtwErrPinNotCorrect);
    err[-5] = LOCALIZE(rtwErrPinBlocked);
    err[-6] = LOCALIZE(rtwErrPinLength);
    err[-7] = LOCALIZE(rtwErrPinReject);
    err[-10] = LOCALIZE(rtwErrArgs);
    err[-11] = LOCALIZE(rtwErrArgsLength);
    err[-12] = LOCALIZE(rtwErrPinWindow);
    err[-20] = LOCALIZE(rtwErrContainerAbsent);
    err[-21] = LOCALIZE(rtwErrNoContainer);
    err[-22] = LOCALIZE(rtwErrContainerDamaged);
    err[-30] = LOCALIZE(rtwErrWrongECP);
    err[-40] = LOCALIZE(rtwErrNoMemory);
    err[-50] = LOCALIZE(rtwErrNoLib);
    err[-51] = LOCALIZE(rtwErrLibNotInit);
    err[-52] = LOCALIZE(rtwErrLibNotSupport);
    err[-53] = LOCALIZE(rtwErrPKCS);

    // ****************************** объект для функций работы с токеном
    var tokenfuncs = {
        // проверка плагина и токена
        tokenIsOk: function () {
            try {
                switchForm(true);
                var res = g.token.rtwIsTokenPresentAndOK();
                g.token.status = res;
                if (res === true) {
                    setNoTokenMessage(true);
                    g.token.rtwLogout();
                    return true; // все ок
                } else {
                    setNoTokenMessage(res);
                    return false; // ошибка на токене
                }
            } catch (e) {
                setNoTokenMessage(0);
                switchForm(false);
                g.token.status = false;
                return false; // плагин не установлен или отключен
            }
        }
    };

    // display toggle
    function dispToggle(ctrl, show) {
        if (ctrl) {
            ctrl.style.display = show ? '' : 'none';
        }
    }

    // сообщение о нерабочем токене
    function setNoTokenMessage(text) {
        g.rtwErrorMessage.innerHTML = err[text] + (text == -1 ? ' <br />' + LOCALIZE(rtwConnectToken) + '.' : '');
    }

    // сообщение на страницу
    function setMessage(text, hide, errmessage) {
        var ctrl = !errmessage ? g.rtwMessage : g.rtwErrorMessage;
        ctrl.innerHTML = text;
        dispToggle(ctrl, true);
        if (hide) {
            setTimeout(function () {
                fadeout(ctrl, 99);
            }, 3000);
        }
    }



    // переключения активности кнопок
    function switchForm(disable) {
        for (var i = 0, l = g.rtwAll.controls.length; i < l; i++) {
            g.rtwAll.controls[i].disabled = !disable;
        }

    }




    //  $grd_ctrls - глобальная переменная для всех контролов, объявляется контролом asp.net
    //************** controls handle ********************
    if (typeof ($grd_ctrls) === 'undefined') throw new Error("define global control!", 0);
    var g = $grd_ctrls;

    /* g.rtwEnable - список токенов с вкл/выкл 
    *  g.rtwConnect - кнопка привязать
    *  g.rtwRemove - Отключить токен
    *  g.rtwUser - Имя пользователя, для имени контейнера

    *  g.rtwUsers - селект логинов
    *  g.rtwRepair - ввод ключа восстановления
    *  g.rtwLogin - кнопка входа
    *  g.rtwRepairBtn - кнопка восстановления доступа
    *  g.rtwFormSign - кнопка подписи формы
    *  g.rtwFormFields - контейнер текстбоксов формы

    *  g.rtwAjaxImg - картинка для статуса аякс загрузки
    *  g.rtwMessage - вывод инфы
    *  g.rtwErrorMessage - вывод ошибок
    *  g.token - token object
    *  g.token.status - статус ключа
    *  g.http - XMLHttpRequest
    *  g.messagelbl - элемент для отображения текста

    */



    if (g.rtwAll == null) return;

    g.rtwAll.controls = g.rtwAll.getElementsByTagName('input');

    // pre to version 2
    g.token_deviceID = function () {
        return g.token.rtwGetPublicKey('repair key').substring(0, 8);
    };

    // **************************************************** Администрирование
    switch (g.controlType) {
        case 'Administration':
            // ******************************** привязать токен
            addEvent(g.rtwConnect, 'click', function () {
                if (g.token.rtwIsUserLoggedIn() === true) {
                    g.token.rtwLogout();
                }
                var loginpin = g.token.rtwUserLoginDlg();
                if (loginpin !== true) {
                    setMessage(err[loginpin], true, true);
                    return;
                }
                var pkey = g.token.rtwGenKeyPair(g.rtwUser);

                if (pkey != -7 && pkey != -12) {
                    if (pkey < 0 && pkey != -21) {
                        setMessage(err[pkey], true, true);
                        g.token.rtwLogout();
                    } else {
                        var rkey = g.token.rtwGetPublicKey('repair key');
                        pkey = g.token.rtwGetPublicKey(g.rtwUser);
                        var id = g.token_deviceID();
                        sendRequest({
                            act: 'attach',
                            pkey: pkey,
                            rkey: rkey,
                            user: id
                        }, attachCallback, errCallback, []);
                    }
                } else {
                    g.token.rtwLogout();
                }
            });

            function attachCallback() {
                var r = parseResponse(this.responseText);
                if (r.type === 'Error') {
                    errCallback(LOCALIZE(rtwErrConnect) + '<br />' + r.text);
                    g.token.rtwDestroyContainer(g.rtwUser);
                } else if (r.type === 'Notify') {
                    setMessage(LOCALIZE(rtwTokenConnected), true);
                    setTimeout(function () {
                        location.reload();
                    }, 500);

                }
                g.token.rtwLogout();
            }

            ;


            //******************************* переключение / отключение токена
            // переключение токена
            addEvent(g.rtwEnable, 'click', function (event) {
                event = event || window.event;
                var t = event.target || event.srcElement;


                if (t.tagName.toLowerCase() === 'input') {
                    var reqparams = {
                        act: t.getAttribute('act'),
                        user: t.getAttribute('token')
                    };
                    var to = t.getAttribute('to');
                    if (to) {
                        reqparams.to = to;
                    }
                    sendRequest(reqparams, switchCallback, errCallback, [t]);
                }
            });

            // обработчик ответа при переключении
            function switchCallback(t) {

                var r = parseResponse(this.responseText);
                if (r.type === 'Error') errCallback(r.text);
                else if (r.type === 'Notify') {
                    if (t.getAttribute('act') === 'remove') {
                        location.reload();
                    }
                    setMessage(LOCALIZE(rtwAuthByToken) + ' ' + (r.text == 'True' ? LOCALIZE(rtwAuthOn) : LOCALIZE(rtwAuthOff)), true);
                    var onoff = r.text == 'True';
                    t.setAttribute('value', onoff ? LOCALIZE(rtwSetAuthOff) : LOCALIZE(rtwSetAuthOn));
                    t.setAttribute('to', onoff ? 'False' : 'True');
                    t.parentNode.previousSibling.innerHTML = onoff ? '✓' : '-';
                }
            }


            //************************************************
            break;


        case 'Login':
        case 'Remember':
            // поле ввода кода восстановления
            addEvent(g.rtwRepair, 'keyup', function (e) {
                this.value = this.value.toUpperCase().replace(/[^A-Z]/g, '').replace(/[A-Z]{4}/g, '$&-').substring(0, 79);
                if (this.createTextRange) { // ie
                    var r = this.createTextRange();
                    r.collapse(false);
                    r.select();
                }
                if (this.selectionStart) { // moz
                    this.setSelectionRange(this.value.length, this.value.length);
                }
            });

            // запрос на авторизацию
            addEvent(g.rtwLogin, 'click', function () {
                if (tokenfuncs.tokenIsOk() && g.rtwUsers.value !== null) {
                    var user = g.rtwUsers.value;
                    sendRequest({
                        act: 'rnd',
                        user: g.token_deviceID()
                    }, rndCallback, errCallback, [user]);
                }
            });

            // запрос на восстановление
            addEvent(g.rtwRepairBtn, 'click', function () {

                if (tokenfuncs.tokenIsOk() && g.rtwRepairUser.value !== '') {
                    var user = g.rtwRepairUser.value;
                    sendRequest({
                        act: 'rnd',
                        user: user
                    }, rndCallback, errCallback, [user]);
                } else {
                    setMessage(LOCALIZE(rtwErrFillLogin), false, true);
                }

            });


            function rndCallback(user) {
                var r = parseResponse(this.responseText);
                if (r.type === 'Error') errCallback(r.text);
                else if (r.type === 'Notify') {
                    tokenSign(r.text, user);
                }
            }



            // подписываем сообщение
            function tokenSign(text, user) {
                if (tokenfuncs.tokenIsOk()) {
                    var urnd = Sha256.hash(Math.random().toString(16));
                    var linkedhash = Sha256.hash(urnd + ':' + text);
                    var sign = !g.repair ? g.token.rtwSign(user, linkedhash) : g.token.rtwRepair(g.rtwRepair.value.replace(/-/g, ''), linkedhash); // если восстановление
                    if (sign <= 0) {
                        setMessage(err[sign], true, true);
                    } else {
                        sendRequest({
                            act: 'login',
                            login: g.repair ? user : user.substring(0, user.indexOf('#%#')),
                            urnd: urnd,
                            sign: sign,
                            repair: g.repair ? true : false,
                            user: g.repair ? 1 : g.token_deviceID()
                        }, loginCallback, errCallback, []);
                    }


                }
            }


            // колбэк логина
            function loginCallback() {
                var r = parseResponse(this.responseText);
                if (r.type === 'Error') errCallback(r.text);
                else if (r.type === 'Notify' && r.text == 'True') {

                    document.location.href = decodeURIComponent(r.url);

                }
            }

            break;
        case "FormSign":


            dispToggle(g.rtwAjaxImg, false);

            break;
        default:
            break;
    }

    this.formSign = function () {
        if (!tokenfuncs.tokenIsOk()) return false;
        if (g.rtwUser === null) {
            setMessage('no user!', true, false);
            return false;
        }
        // получаем все текстовые поля  и селекты
        var fields = [];
        var inputs = g.rtwFormFields.getElementsByTagName('input');
        var textareas = g.rtwFormFields.getElementsByTagName('textarea');
        var selects = g.rtwFormFields.getElementsByTagName('select');
        for (var i = 0, l1 = inputs.length; i < l1; i++) {
            if (inputs[i].getAttribute('type') === 'text' || inputs[i].getAttribute('type') === 'password') {
                fields.push(inputs[i]);
            }
        }
        for (var j = 0, l2 = textareas.length; j < l2; j++) {
            fields.push(textareas[j]);
        }
        for (var l = 0, l4 = selects.length; l < l4; l++) {
            fields.push(selects[l]);
        }

        fields.sort(nodesort);
        console.log(fields);
        var textToSign = '';
        for (var k = 0, l3 = fields.length; k < l3; k++) {
            textToSign += fields[k].value;
        }

        console.log(textToSign);
        return tokenSignForm(textToSign, g.rtwUser);


    };

    function tokenSignForm(text, user) {
        if (tokenfuncs.tokenIsOk()) {

            var hash = Sha256.hash(text);
            var sign = g.token.rtwSign(user, hash);
            if (sign <= 0) {
                setMessage(err[sign], true, true);
                return false;
            } else {
                g.rtwTokenId.value = g.token_deviceID();
                g.rtwFormSignTxt.value = sign;
                return true;
            }
        }
        return false;
    }

    function nodesort(a, b) {
        return a.getAttribute('id').localeCompare(b.getAttribute('id'));
        return a.getAttribute('id') > b.getAttribute('id') ? 1 : -1;
    }

    this.testToken = function () {
        tokenRefresh();
    };


    // обновление инфы в контролах с логинами
    function tokenRefresh() {

        if (!g.rtwUsers) return;
        for (var i = g.rtwUsers.options.length - 1; i >= 0; g.rtwUsers.remove(i), i--) {
        }
        if (tokenfuncs.tokenIsOk()) {
            var containerCount = g.token.rtwGetNumberOfContainers();
            if (containerCount == 0) {
                g.rtwUsers.disabled = true;
                setMessage(LOCALIZE(rtwErrNoLoginsOnToken), false, true);
            } else {
                g.rtwUsers.disabled = false;
                for (i = 0; i < containerCount; i++) {
                    var contName = g.token.rtwGetContainerName(i);
                    g.rtwUsers.options[i] = new Option(contName.replace("#%#", " - "), contName);
                }
                if (g.rtwLogin && g.rtwLogin.offsetHeight != 0) {
                    g.rtwLogin.focus();
                }

            }


        } else if (g.token.status == -1) { // токен не вставлен
            checkTokenTimeout(false);
        }
    }



    // если токен не воткнут - запускаем 
    var timeoutid;

    function checkTokenTimeout(stop) {
        if (!stop) {
            dispToggle(g.rtwUsers, false);
            timeoutid = setTimeout(function () { checkTokenTimeout(stop); }, 50);
            stop = tokenfuncs.tokenIsOk();
        } else {
            clearTimeout(timeoutid);
            dispToggle(g.rtwUsers, true);
            tokenRefresh();
        }
    }




    //--------------------================== start ==================--------------------------
    tokenfuncs.tokenIsOk();
    tokenRefresh();



};