import { Injectable, inject } from "@angular/core";
import { LightNapWebApiService } from "@core/backend-api/services/lightnap-api";

@Injectable({
  providedIn: "root",
})
export class PublicService {
    #webApiService = inject(LightNapWebApiService);
}
