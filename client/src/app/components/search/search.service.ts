import { Injectable } from '@angular/core';
import { Http, URLSearchParams } from '@angular/http';
import 'rxjs/add/operator/toPromise';

import { Item } from '../../common/models';
import { Settings } from '../../common/settings';

@Injectable()
export class SearchService {
  private endpoint: string;

  constructor(private http: Http, settingsService: Settings) {
    this.endpoint = `${settingsService.apiBaseUrl}/search`;
  }

  public performSearch(searchTerm: string): Promise<Item[]> {
    if (!searchTerm || searchTerm.length === 0) {
      return undefined;
    }

    const params = new URLSearchParams();
    params.set('q', searchTerm);

    return this.http.get(this.endpoint, { search: params })
      .toPromise()
      .then(response => {
        if (response.status === 204) {
          return undefined;
        } else {
          const result = response.json();
          return result.results;
        }
      });
  }
}