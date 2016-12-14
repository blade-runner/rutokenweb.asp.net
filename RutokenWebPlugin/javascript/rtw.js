Function.prototype.bind = function (scope) {
    var fn = this;
    return function () {
        return fn.apply(scope, arguments);
    };
};


if (typeof String.prototype.trim !== 'function') {
	String.prototype.trim = function() {
		return this.replace(/^\s+|\s+$/g, '');
	};
}


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

    var errorNode = document.getElementById('rtwErrorMessage');
        rutokenweb.ready.then(function () {
            if (window.chrome) {
                return rutokenweb.isExtensionInstalled();
            } else {
                return Promise.resolve(true);
            }
        }).then(function (result) {
            if (result) {
                return rutokenweb.isPluginInstalled();
            } else {
                throw "Установите расширение для Рутокен Web - <a href='https://chrome.google.com/webstore/detail/%D0%B0%D0%B4%D0%B0%D0%BF%D1%82%D0%B5%D1%80-%D1%80%D1%83%D1%82%D0%BE%D0%BA%D0%B5%D0%BD-web-%D0%BF%D0%BB%D0%B0%D0%B3%D0%B8/boabkkhbickbpleplbghkjpcoebckgai' target='_blank'>Адаптер Рутокен Web Плагин</a>";
            }
        }).then(function (result) {
            if (result) {
                return rutokenweb.loadPlugin();
            } else {
                throw "Установите плагин для работы с Рутокен Web";
            }
        }).then(function (plugin) {
            console.log(plugin);
            $GrdToken = new Grd(plugin);
        }).then(undefined, function (reason) {
            errorNode.innerHTML = reason;
            console.log(reason);
        });




 
});



var Grd = function (plugin) {


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
    })(window, document);


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
                    throw new Error("Ошибка ajax запроса");
            }


        };
    }




    // запрос на сервер
    function sendRequest(httpParams, callback, onerror, callbackArgsArray, method) {
    	method = method || 'GET';
        var xmlHttpRequest = new (window.XMLHttpRequest || ActiveXObject)("Msxml2.XMLHTTP");
        if (!xmlHttpRequest)
            throw new Error("XMLHttpRequest error");
        switchForm(false);
        xmlHttpRequest.onreadystatechange =
            registreCallbackFunction(xmlHttpRequest, callback, onerror, callbackArgsArray);
        try {
            dispToggle(g.rtwAjaxImg, true);
            xmlHttpRequest.open('get', encodeURI(g.settings.mainurl + "?" + getHttpParamsString(httpParams)), true);
            xmlHttpRequest.setRequestHeader('X-Requested-With', 'XhrRutoken');
            xmlHttpRequest.send(null);
        } catch (e) {
            dispToggle(g.rtwAjaxImg, false);
            xmlHttpRequest.onreadystatechange = null;
            if (typeof onerror == "function")
                onerror.apply(xmlHttpRequest, callbackArgsArray);
            else
                throw new Error("Ошибка ajax запроса");
        }
    }

    var Win1251ToHEX = function (str) {
        var Win1251 =
                    {
                        0x402: 0x80,
                        0x403: 0x81,
                        0x201A: 0x82,
                        0x453: 0x83,
                        0x201E: 0x84,
                        0x2026: 0x85,
                        0x2020: 0x86,
                        0x2021: 0x87,
                        0x20AC: 0x88,
                        0x2030: 0x89,
                        0x409: 0x8A,
                        0x2039: 0x8B,
                        0x40A: 0x8C,
                        0x40C: 0x8D,
                        0x40B: 0x8E,
                        0x40F: 0x8F,
                        0x452: 0x90,
                        0x2018: 0x91,
                        0x2019: 0x92,
                        0x201C: 0x93,
                        0x201D: 0x94,
                        0x2022: 0x95,
                        0x2013: 0x96,
                        0x2014: 0x97,
                        0x2122: 0x99,
                        0x459: 0x9A,
                        0x203A: 0x9B,
                        0x45A: 0x9C,
                        0x45C: 0x9D,
                        0x45B: 0x9E,
                        0x45F: 0x9F,
                        0xA0: 0xA0,
                        0x40E: 0xA1,
                        0x45E: 0xA2,
                        0x408: 0xA3,
                        0xA4: 0xA4,
                        0x490: 0xA5,
                        0xA6: 0xA6,
                        0xA7: 0xA7,
                        0x401: 0xA8,
                        0xA9: 0xA9,
                        0x404: 0xAA,
                        0xAB: 0xAB,
                        0xAC: 0xAC,
                        0xAD: 0xAD,
                        0xAE: 0xAE,
                        0x407: 0xAF,
                        0xB0: 0xB0,
                        0xB1: 0xB1,
                        0x406: 0xB2,
                        0x456: 0xB3,
                        0x491: 0xB4,
                        0xB5: 0xB5,
                        0xB6: 0xB6,
                        0xB7: 0xB7,
                        0x451: 0xB8,
                        0x2116: 0xB9,
                        0x454: 0xBA,
                        0xBB: 0xBB,
                        0x458: 0xBC,
                        0x405: 0xBD,
                        0x455: 0xBE,
                        0x457: 0xBF,
                        0x410: 0xC0,
                        0x411: 0xC1,
                        0x412: 0xC2,
                        0x413: 0xC3,
                        0x414: 0xC4,
                        0x415: 0xC5,
                        0x416: 0xC6,
                        0x417: 0xC7,
                        0x418: 0xC8,
                        0x419: 0xC9,
                        0x41A: 0xCA,
                        0x41B: 0xCB,
                        0x41C: 0xCC,
                        0x41D: 0xCD,
                        0x41E: 0xCE,
                        0x41F: 0xCF,
                        0x420: 0xD0,
                        0x421: 0xD1,
                        0x422: 0xD2,
                        0x423: 0xD3,
                        0x424: 0xD4,
                        0x425: 0xD5,
                        0x426: 0xD6,
                        0x427: 0xD7,
                        0x428: 0xD8,
                        0x429: 0xD9,
                        0x42A: 0xDA,
                        0x42B: 0xDB,
                        0x42C: 0xDC,
                        0x42D: 0xDD,
                        0x42E: 0xDE,
                        0x42F: 0xDF,
                        0x430: 0xE0,
                        0x431: 0xE1,
                        0x432: 0xE2,
                        0x433: 0xE3,
                        0x434: 0xE4,
                        0x435: 0xE5,
                        0x436: 0xE6,
                        0x437: 0xE7,
                        0x438: 0xE8,
                        0x439: 0xE9,
                        0x43A: 0xEA,
                        0x43B: 0xEB,
                        0x43C: 0xEC,
                        0x43D: 0xED,
                        0x43E: 0xEE,
                        0x43F: 0xEF,
                        0x440: 0xF0,
                        0x441: 0xF1,
                        0x442: 0xF2,
                        0x443: 0xF3,
                        0x444: 0xF4,
                        0x445: 0xF5,
                        0x446: 0xF6,
                        0x447: 0xF7,
                        0x448: 0xF8,
                        0x449: 0xF9,
                        0x44A: 0xFA,
                        0x44B: 0xFB,
                        0x44C: 0xFC,
                        0x44D: 0xFD,
                        0x44E: 0xFE,
                        0x44F: 0xFF
                    };
        var o1, o2, c, coded;
        coded = '';
        for (c = 0; c < str.length; c++) {
            o2 = str.charCodeAt(c);
            o1 = o2 < 128 ? o2 : Win1251[o2];
            if (o1 == null && o2 > 0)
                o1 = 63; 
            coded += ((o1 & 0xf0) >> 4).toString(16);
            o1 = o1 & 0x0f;
            coded += o1.toString(16);
        }
        return coded;
    };

    var rvalidchars = /^[\],:{}\s]*$/,
        rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g,
        rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g,
        rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g;

    function parseResponse(data) {

        if (typeof data !== "string" || !data) {
            return { type: 'Error', text: "Ошибка запроса. Возможно вы не авторизованы" };
        }

        if (window.JSON && window.JSON.parse) {


            return window.JSON.parse(data.trim());
        }
        // Logic borrowed from http://json.org/json2.js
        if (rvalidchars.test(data.replace(rvalidescape, "@")
                .replace(rvalidtokens, "]")
                .replace(rvalidbraces, ""))) {

            return (new Function("return " + data))();

        }
        return { type: 'Error', text: "Ошибка запроса. Возможно вы не авторизованы" };
    }


    // общий обработчик для ошибок
    function errCallback(text) {
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
    err[0] = "Плагин не установлен или отключен";
    err[-1] = "USB-токен не найден.";
    err[-2] = "USB-токен не залогинен пользователем";
    err[-3] = "PIN-код не верен";
    err[-4] = "PIN-код не корректен";
    err[-5] = "PIN-код заблокирован";
    err[-6] = "Неправильная длина PIN-кода";
    err[-7] = "Отказ от ввода PIN-кода";
    err[-10] = "Неправильные аргументы функции";
    err[-11] = "Неправильная длина аргументов функции";
    err[-12] = "Открыто другое окно ввода PIN-кода";
    err[-20] = "Контейнер не найден";
    err[-21] = "Контейнер уже существует";
    err[-22] = "Контейнер поврежден";
    err[-30] = "ЭЦП не верна";
    err[-40] = "Не хватает свободной памяти чтобы завершить операцию";
    err[-50] = "Библиотека не загружена";
    err[-51] = "Библиотека находится в неинициализированном состоянии";
    err[-52] = "Библиотека не поддерживает расширенный интерфейс";
    err[-53] = "Ошибка в библиотеке rtpkcs11ecp";

    // ****************************** объект для функций работы с токеном
    var tokenfuncs = {
        // проверка плагина и токена
        tokenIsOk: function () {

            return new Promise(function(resolve, reject) {
                if (g.controlType == 'Remember') resolve(true);
                try {
                    switchForm(true);
                   g.token.rtwIsTokenPresentAndOK().then(function(res) {
                       g.token.status = res;
                       if (res === true) {
                           setNoTokenMessage(true);
                           //  g.token.rtwLogout();
                           resolve(true);
                       } else {
                           setNoTokenMessage(-1);
                           reject(false);
                       }
                   });
                    
                  
                } catch (e) {
                    setNoTokenMessage(0);
                    switchForm(false);
                    g.token.status = false;
                    reject(false);
                }
            });


          
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
        g.rtwErrorMessage.innerHTML = err[text] + (text == -1 ? ' <br />' + "Подключите токен" + '.' : '');
    }

    // сообщение на страницу
    function setMessage(text, hide, errmessage, timeout) {
        timeout = timeout || 3000;
        var ctrl = !errmessage ? g.rtwMessage : g.rtwErrorMessage;
        ctrl.innerHTML = text;
        dispToggle(ctrl, true);
        if (hide) {
            setTimeout(function () {
                fadeout(ctrl, 99);
            }, timeout);
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
    g.token = plugin;

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

    var css = '* {background-color: transparent}' +
    'QPushButton { color: #404040; border-style: solid; border-color: #888888; border-width: 1px; border-radius: 5px; min-width: 75px; min-height: 25px}' +
   'QPushButton:hover { background-color: #efefef}' +
    'QLabel { color: #404040}' +
    'QLineEdit {color: #404040}' +
   ' QDialog {background-color: white }';

    css = '';

  


    // вывод сообщения об ошибке
    function returnError(errorCode) {
        setMessage(err[errorCode.message] || errorCode, true, true);
    }

    // **************************************************** Администрирование
    switch (g.controlType) {
        case 'Administration':
            // ******************************** привязать токен
            addEvent(g.rtwConnect, 'click', function () { rtwConnectClick(); });


            function rtwConnectClick() {


                // пробуем получить repairkey
                g.token.rtwGetPublicKey('repair key').then(function(rkey) {
                    return g.token.rtwGenKeyPair(g.rtwUser, css);
                }).then(returnGen, returnGenerateError);

                
              


            }


            function returnGenerateError(errorcode) {
                if (errorcode.message == -21) {
                    setMessage("На токене есть контейнер с вашим именем пользователя. Для корректной привязки токена попробуем удалить контейнер и созать его заново.", true, true, 5000);
                    setTimeout(function () {
                        g.token.rtwDestroyContainer(g.rtwUser, css).then(rtwConnectClick, returnError);
                    }, 2000);
                } else if (errorcode.message == -20) {
                    setMessage("Вы используете токен без контейнера восстановления. Возможно это не Рутокен Web.", false, true);
                } else {
                    returnError(errorcode);
                }

            }



            function returnGen(publicKey) {
                var rkey;
                g.token.rtwGetPublicKey('repair key')
                .then(function (repairKey) {
                    rkey = repairKey;
                    return g.token.rtwGetDeviceID();
                 }).then(function(deviceId) {
                     sendRequest({
                         act: 'attach',
                         pkey: publicKey,
                         rkey: rkey,
                         user: deviceId
                     }, attachCallback, errCallback, []);
                    });

           
                

             



                
            }




            function attachCallback() {
                var r = parseResponse(this.responseText);

                if (r.type === 'Error') {

            
                        g.token.rtwDestroyContainer(g.rtwUser, css).then(function (message) { returnDestroy(message); }.bind(r), returnError);
                  


                } else if (r.type === 'Notify') {
                    setMessage("Токен привязан к аккаунту", true);
                    setTimeout(function () {
                        location.reload();
                    }, 500);

                }

            }

            function returnDestroy(message) {
                errCallback("Привязка не удалась" + '<br />' + message);
            }


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
                    setMessage("Аутентификация по токену" + ' ' + (r.text == 'True' ? "включена" : "выключена"), true);
                    var onoff = r.text == 'True';
                    t.setAttribute('value', onoff ? "Отключить" : "Включить");
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

                
                var keyCode = e.keyCode ? e.keyCode : e.which;

                if (keyCode != 8) {
                    this.value = this.value.toUpperCase().replace(/[^A-Z]/g, '').replace(/[A-Z]{4}/g, '$&-').substring(0, 79);
                    if (this.createTextRange) { // ie
                        var r = this.createTextRange();
                        r.collapse(false);
                        r.select();
                    }
                    if (this.selectionStart) { // moz
                        this.setSelectionRange(this.value.length, this.value.length);
                    }
                }
               
            });

            // запрос на авторизацию
            addEvent(g.rtwLogin, 'click', function () {
                tokenfuncs.tokenIsOk().then(function() {
                    if (g.rtwUsers.value !== null) {
                        var user = g.rtwUsers.value;
                        g.token.rtwGetDeviceID().then(function(id) {
                            sendRequest({
                                act: 'rnd',
                                user: id
                            }, rndCallback, errCallback, [user]);
                        });

                       
                    }
                });


               
            });

            // запрос на восстановление
            addEvent(g.rtwRepairBtn, 'click', function () {

                if (g.rtwRepairUser.value !== '') {
                    var login = g.rtwRepairUser.value;
                    sendRequest({
                        act: 'rnd',
                        login: login,
                        repair: true
                    }, rndCallback, errCallback, [login]);
                } else {
                    setMessage("Заполните поле логин", false, true);
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
                tokenfuncs.tokenIsOk().then(function() {
                        var urnd = Sha256.hash(Math.random().toString(16));
                        var linkedhash = Sha256.hash(urnd + ':' + text);


                        if (!g.repair) {

                            g.token.rtwSign(user, linkedhash, css).then(function(signature) {
                                g.token.rtwGetDeviceID().then(function(id) {
                                    returnSign(signature, urnd, user, id);
                                });
                            }, returnError);
                        } else {


                            g.token.rtwRepair(g.rtwRepair.value.replace(/-/g, ''), linkedhash, undefined).then(function(sign) {
                                if (sign <= 0) {
                                    setMessage(err[sign], true, true);
                                } else {
                                    returnSign(signature, urnd, user, 1);
                                }
                            });


                        }
                    }
                );
            }
            

            function returnSign(sign, urnd, user, id) {
                sendRequest({
                    act: 'login',
                    login: g.repair ? user : user.substring(0, user.indexOf('#%#')),
                    urnd: urnd,
                    sign: sign,
                    repair: g.repair ? true : false,
                    user: id
                }, loginCallback, errCallback, []);
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



       
        if (g.rtwUser === null) {
            setMessage('Пользователь не выбран', true, false);
            return false;
        }


        g.token.rtwGetDeviceID().then(function(id) {
            sendRequest({
                act: 'rnd',
                user: id
            }, rndFormCallback, errCallback, [g.rtwUser]);
        });


       
        function rndFormCallback(user) {

            

            var r = parseResponse(this.responseText);

            

            if (r.type === 'Error') errCallback(r.text);
            else if (r.type === 'Notify') {

                // подписываем данные формы и рандом
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
                var textToSign = '';
                for (var k = 0, l3 = fields.length; k < l3; k++) {
                    textToSign += '<N>'+ fields[k].getAttribute('pinpadfield') + '<V>'+ fields[k].value;
                }

             
                textToSign = '<!PINPADFILE RU>' + textToSign + '<!>' + r.text;
            

               

                tokenSignForm(textToSign, g.rtwUser);
            }
        }


       return false;

        


    };

    //function tokenSignForm(text, user) {
    //    if (tokenfuncs.tokenIsOk()) {
    //        g.token.rtwHashSign(user, text,css,returnFormSign,returnError);
    //    }

    //}

    //function returnFormSign(sign) {
    //    g.rtwTokenId.value = g.token_p.rtwGetDevice_ID();
    //    g.rtwFormSignTxt.value = sign;
    //    rtwSendSignedFrom();

    //}
    




    function tokenSignForm(text, user) {
        tokenfuncs.tokenIsOk().then(function() {
            var ctext = Win1251ToHEX(text);


            g.token.rtwHashSign(user, ctext, css).then(function(sign) {
                if (sign <= 0) {
                    setMessage(err[sign], true, true);
                } else {

                    g.token.rtwGetDeviceID().then(function(id) {
                        g.rtwTokenId.value = id;
                        g.rtwFormSignTxt.value = sign;
                        g.rtwFormSignData.value = ctext;
                        g.rtwFormSend.style.display = '';
                    });

                  

                }
            });
           
        });


           
     
       
        
       
    }




    function nodesort(a, b) {
        return a.getAttribute('pinpadfield').toLowerCase().localeCompare(b.getAttribute('pinpadfield').toLowerCase());
    }

    this.testToken = function () {
        tokenRefresh();
    };




    // обновление инфы в контролах с логинами
    function tokenGetInfo() {
      
        g.token.rtwGetNumberOfContainers()
            .then(function (containerCount) {
                if (containerCount === 0) {
                    g.rtwUsers.disabled = true;
                    throw new Error('"На токене не зарегистрированы учетные записи. Привяжите токен к логину в личном кабинете."');
                    //setMessage("На токене не зарегистрированы учетные записи. Привяжите токен к логину в личном кабинете.", false, true);

                } else {
                    g.rtwUsers.disabled = false;

                    var promises = [];
                    for (var i = 0; i < containerCount; i++) {
                        promises.push(g.token.rtwGetContainerName(i));
                    }
                    return Promise.all(promises);


                }
            }).then(function(names) {

                names.forEach(function (cont_name, i) {
                    g.rtwUsers.options[i] = new Option(cont_name.replace("#%#", " - "), cont_name);
                });
                if (g.rtwLogin && g.rtwLogin.offsetHeight !== 0) {
                    g.rtwLogin.focus();
                }
            });


    }

    function tokenRefresh() {

        if (!g.rtwUsers) return;
        for (var i = g.rtwUsers.options.length - 1; i >= 0; g.rtwUsers.remove(i), i--) {
        }
        tokenfuncs.tokenIsOk().then(function(res) {
            dispToggle(g.rtwUsers, true);
            tokenGetInfo();
        }).then(undefined, function() {
            dispToggle(g.rtwUsers, false);
            setTimeout(function () { tokenRefresh(); }, 1000);
        });
    } 
   

   



    //--------------------================== start ==================--------------------------



        tokenRefresh();
   
    
      
   
   



};