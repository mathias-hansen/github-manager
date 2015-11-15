var Github = (function () {
    function Github(accessToken) {
        this.accessToken = accessToken;
        this.xhr = new Xhr();
    }
    Github.prototype.getUser = function () {
        return this.apiRequest("GET", "/user");
    };
    Github.prototype.getOrgs = function () {
        return this.apiRequest("GET", "/user/orgs");
    };
    Github.prototype.getRepos = function (user) {
        return this.apiRequest("GET", "/users/" + user + "/repos");
    };
    Github.prototype.getHooks = function (owner, repo) {
        return this.apiRequest("GET", "/repos/" + owner + "/" + repo + "/hooks");
    };
    Github.prototype.addHook = function (owner, repo) {
        var data = {
            name: "web",
            active: true,
            config: {
                url: "http://localhost:5000/webhook",
                content_type: "json"
            }
        };
        return this.apiRequest("POST", "/repos/" + owner + "/" + repo + "/hooks", data);
    };
    Github.prototype.getHookId = function (owner, repo) {
        return this.getHooks(owner, repo).then(function (hooks) {
            hooks = hooks.filter(function (hook) { return hook.config.url === "http://localhost:5000/webhook"; });
            if (hooks.length > 0) {
                return hooks[0].id;
            }
            else {
                return null;
            }
        });
    };
    Github.prototype.removeHook = function (owner, repo, id) {
        return this.apiRequest("DELETE", "/repos/" + owner + "/" + repo + "/hooks/" + id);
    };
    Github.prototype.apiRequest = function (method, url, data) {
        return this.xhr.request(method, "https://api.github.com" + url, { "Authorization": "token " + this.accessToken }, data);
    };
    Github.prototype.search = function (owner, orgs, query) {
        var _this = this;
        var items = [this.apiRequest("GET", "/search/repositories?q=" + query + " user:" + owner)];
        orgs.forEach(function (org) {
            items.push(_this.apiRequest("GET", "/search/repositories?q=" + query + " user:" + org));
        });
        return items;
    };
    return Github;
})();
var Xhr = (function () {
    function Xhr() {
    }
    Xhr.prototype.request = function (method, url, headers, data) {
        var xhr = new XMLHttpRequest();
        xhr.open(method, url, true);
        for (var header in headers) {
            xhr.setRequestHeader(header, headers[header]);
        }
        return new Promise(function (resolve, reject) {
            xhr.onload = resolve;
            xhr.onerror = reject;
            xhr.send(JSON.stringify(data));
        }).then(this.parseResponse.bind(this));
    };
    Xhr.prototype.parseResponse = function (event) {
        var response = event.target.response;
        try {
            return JSON.parse(response);
        }
        catch (event) {
            return response;
        }
    };
    return Xhr;
})();
function selectUpPathElement(elementName, path) {
    var selected = null;
    elementName = elementName.toLocaleLowerCase();
    for (var _i = 0, path_1 = path; _i < path_1.length; _i++) {
        var element = path_1[_i];
        var name = element.nodeName.toLocaleLowerCase();
        if (name == elementName) {
            selected = element;
            break;
        }
    }
    ;
    return selected;
}
