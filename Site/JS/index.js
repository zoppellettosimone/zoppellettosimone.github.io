function TCGSite() {

    var Me = this;

    Me.Mtg = {};
    Me.YuGiOh = {};
    Me.Pokemon = {};

    Me.httpRequestAsync = function (requestType, url, callback) {

        var xmlHttp = new XMLHttpRequest();
        xmlHttp.onreadystatechange = function () {
            console.debug(xmlHttp)
            if (xmlHttp.readyState == 4 && xmlHttp.status == 200)
                callback(xmlHttp.responseText);
        }
        xmlHttp.open(requestType.toUpperCase(), url, true); // true for asynchronous 
        xmlHttp.withCredentials = true;
        xmlHttp.send();

    }

    Me.initPage = function () {

        console.debug("Start Load Page");

        Me.httpRequestAsync("GET", "localhost:8000/iAmLive", function (responseText) {

            console.debug(responseText);

        });

    }



}

var $TCGSite = new TCGSite;
