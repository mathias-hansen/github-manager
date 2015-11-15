interface User
{
	id: number;
	login: string;
	name: string;
	email: string;
	blog: string;
	avatar_url: string;
}

interface Org 
{
	id: number;
	login: string;
	description: string;
	avatar_url: string;
}

interface Repo
{
	id: number;
	owner: {
		id: number;
		login: string;
		avatar_url: string;
	}
	name: string;
	full_name: string;
	description: string;
	private: boolean;
	fork: boolean;
	homepage: string;
	language: string;
	default_branch: string;
	open_issues_count: number;
	permissions: {
      	admin: boolean,
      	push: boolean,
      	pull: boolean
    }
}

interface Hook
{
	id: number;
	name: string;
	active: boolean;
	config: {
		content_type: string;
		insecure_ssl: string;
		secret: string;
		url: string;
	};
	created_at: string;
	updated_at: string;
	events: string[];
	last_response: {
		code: number;
		message: string;
		status: string;	
	};
}

class Github
{
	xhr: Xhr = new Xhr();
	
	constructor(public accessToken: string) {
		
	}
	getUser(): Promise<User> {
		return this.apiRequest<User>("GET", "/user");
	}
	getOrgs(): Promise<Org[]> {
		return this.apiRequest<Org[]>("GET", "/user/orgs");
	}
	getRepos(user: string): Promise<Repo[]> {
		return this.apiRequest<Repo[]>("GET", `/users/${user}/repos`);
	}
	getHooks(owner: string, repo: string): Promise<Hook[]> {
		return this.apiRequest<Hook[]>("GET", `/repos/${owner}/${repo}/hooks`);
	}
	addHook(owner: string, repo: string): Promise<Hook> {
		var data = {
			name: "web",
			active: true,
			config: {
				url: "http://localhost:5000/webhook",
				content_type: "json"
			}
		};
		
		return this.apiRequest<Hook>("POST", `/repos/${owner}/${repo}/hooks`, data);
	}
	getHookId(owner: string, repo: string): Promise<number> {
		return this.getHooks(owner, repo).then(hooks => {
			hooks = hooks.filter(hook => hook.config.url === "http://localhost:5000/webhook");
			
			if (hooks.length > 0) {
				return hooks[0].id;
			}
			else {
				return null;
			}
		});
	}
	removeHook(owner: string, repo: string, id: number): Promise<void> {
		return this.apiRequest<void>("DELETE", `/repos/${owner}/${repo}/hooks/${id}`);
	}
	apiRequest<T>(method: string, url: string, data?): Promise<T> {
		return this.xhr.request<T>(
			method, 
			"https://api.github.com" + url, 
			{ "Authorization": "token " + this.accessToken },
			data);
	}
	search(owner: string, orgs: string[], query: string): Promise<Repo[]>[] {
		var items = [this.apiRequest<any>("GET", "/search/repositories?q=" + query + " user:" + owner)];

		orgs.forEach(org => {
			items.push(this.apiRequest<any>("GET", "/search/repositories?q=" + query + " user:" + org));
		});

		return items;
	}
}

class Xhr
{
	request<T>(method: string, url: string, headers?, data?): Promise<T | string> {
		var xhr = new XMLHttpRequest();
		
		xhr.open(method, url, true);
		
		for (var header in headers) {
			xhr.setRequestHeader(header, headers[header]);
		}
		
		return new Promise<Event>((resolve, reject) => {
			xhr.onload = resolve;
			xhr.onerror = reject;
			
			xhr.send(JSON.stringify(data));
		}).then(this.parseResponse.bind(this));
	}
	parseResponse<T>(event: Event): T | string {
		var response = (<any>event.target).response;
		
		try {
			return JSON.parse(response);
		} catch (event) {
			return response;
		}
	}
}

function selectUpPathElement(elementName: string, path: HTMLElement[]): HTMLElement {
	var selected = null;
	
	elementName = elementName.toLocaleLowerCase();
	
	for (var element of path) {
		var name = element.nodeName.toLocaleLowerCase();
		
		if (name == elementName) {
			selected = element;
			break;
		}	
	};
	
	return selected;
}