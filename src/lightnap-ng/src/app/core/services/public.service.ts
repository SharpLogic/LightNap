import { Injectable, inject } from "@angular/core";
import { LightNapWebApiService } from "@core/backend-api/index.service";

@Injectable({
  providedIn: "root",
})
export class PublicService {
    #dataService = inject(LightNapWebApiService);
}
