import { Injectable, inject } from "@angular/core";
import { PublicDataService } from "@core/backend-api/services/public-data.service";

@Injectable({
  providedIn: "root",
})
export class PublicService {
    #dataService = inject(PublicDataService);
}
