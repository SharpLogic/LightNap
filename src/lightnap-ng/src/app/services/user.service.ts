import { inject, Injectable } from "@angular/core";
import { UserDataService } from "./user-data.service";

@Injectable({
  providedIn: "root",
})
export class UserService {
  #dataService = inject(UserDataService);
}
