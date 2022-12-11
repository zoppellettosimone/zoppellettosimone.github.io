function TCGSite() {

    var Me = this;

    Me.Mtg = {};
    Me.YuGiOh = {};
    Me.Pokemon = {};

    Me.Mtg.allMtgCards = [];
    Me.YuGiOh.allYuGiOhCards = [];
    Me.Pokemon.allPokemonCards = [];

    Me.Mtg.loadFinish = false;
    Me.YuGiOh.loadFinish = false;
    Me.Pokemon.loadFinish = false;

    Me.httpRequestAsync = function(url, requestType, callback) {

        var xmlHttp = new XMLHttpRequest();
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState == 4 && xmlHttp.status == 200)
                callback(xmlHttp.responseText);
        }
        xmlHttp.open(requestType.toUpperCase(), url, true); // true for asynchronous 
        xmlHttp.send();

    }

    Me.Mtg.RequestMtgCards = function(cardName){
        
        Me.httpRequestAsync("https://api.scryfall.com/cards/search?order=cmc&q=" + cardName.toLowerCase(), "GET", (responseString) => {
            // page content is in variable "res"

            if(responseString != ""){

                var responseJson = JSON.parse(responseString);

                console.debug(responseJson);

                if(responseJson.data.lenght > 0){

                    console.debug(responseJson.data.length);

                    for (let i = 0; i < responseJson.data.length; i++) {

                       console.debug(responseJson.data[i]);
                        
                    }

                }
 
                if(responseJson.has_more == true){

                    console.debug("More")

                }


                //has_more == true -> Request next_page
                //has_more == false -> Finite le pagine

            }

        });

    }

    function RequestAllMtgCards(url){

        Me.httpRequestAsync(url, "GET", (responseString) => {
            // page content is in variable "res"

            if(responseString != ""){

                var responseJson = JSON.parse(responseString);

                if(responseJson.data.length > 0){

                    for (let i = 0; i < responseJson.data.length; i++) {

                        Me.Mtg.allMtgCards.push(responseJson.data[i]);
                        
                    }

                }

                if(responseJson.has_more == true){

                    setTimeout(function(){

                        RequestAllMtgCards(responseJson.next_page);

                    }, 100);

                }else{

                    Me.Mtg.loadFinish = true;

                    console.debug("MTG Finish")


                }


                //has_more == true -> Request next_page
                //has_more == false -> Finite le pagine

            }

        });

    }

    Me.Mtg.searchCards = function(nameCard){
        Me.Mtg.allMtgCards.filter(x => x.name.toLowerCase().includes(nameCard.toLowerCase())) 
    }

    Me.initPage = function () {

        

        console.debug("Start Mtg Load Card");

        RequestAllMtgCards("https://api.scryfall.com/cards/search?order=cmc&q=*");

        /*

        if(Me.Mtg.loadFinish == true){

            Me.Mtg.searchCards("Brainstorm")

        }else{

            Me.Mtg.RequestMtgCards("Brainstorm");

        }

        */

    }

    

}

var $TCGSite = new TCGSite;
