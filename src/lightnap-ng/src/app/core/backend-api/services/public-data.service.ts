import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class PublicDataService {
  #http = inject(HttpClient);
  #apiUrlRoot = "/api/public/";

}
