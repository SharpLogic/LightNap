import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { tap } from "rxjs";
import {
  CreateStaticContentDto,
  CreateStaticContentLanguageDto,
  PagedResponseDto,
  SearchStaticContentRequestDto,
  StaticContentDto,
  StaticContentLanguageDto,
  StaticContentSupportedLanguageDto,
  UpdateStaticContentDto,
  UpdateStaticContentLanguageDto,
} from "../dtos";
import { PublishedStaticContentResultDto } from "../dtos/static-contents/response/published-static-content-result-dto";

@Injectable({
  providedIn: "root",
})
export class ContentDataService {
  #http = inject(HttpClient);
  #apiUrlRoot = "/api/content/";

  getPublishedStaticContent(key: string, languageCode: string) {
    return this.#http.get<PublishedStaticContentResultDto | null>(`${this.#apiUrlRoot}published/${key}/${languageCode}`);
  }

  getSupportedLanguages() {
    return this.#http.get<Array<StaticContentSupportedLanguageDto>>(`${this.#apiUrlRoot}supported-languages`);
  }

  createStaticContent(createDto: CreateStaticContentDto) {
    return this.#http.post<StaticContentDto>(`${this.#apiUrlRoot}`, createDto);
  }

  getStaticContent(key: string) {
    return this.#http.get<StaticContentDto | null>(`${this.#apiUrlRoot}${key}`);
  }

  searchStaticContent(searchDto: SearchStaticContentRequestDto) {
    return this.#http.post<PagedResponseDto<StaticContentDto>>(`${this.#apiUrlRoot}search`, searchDto);
  }

  updateStaticContent(key: string, updateDto: UpdateStaticContentDto) {
    return this.#http.put<StaticContentDto>(`${this.#apiUrlRoot}${key}`, updateDto);
  }

  deleteStaticContent(key: string) {
    return this.#http.delete<boolean>(`${this.#apiUrlRoot}${key}`);
  }

  getStaticContentLanguage(key: string, languageCode: string) {
    return this.#http.get<StaticContentLanguageDto | null>(`${this.#apiUrlRoot}${key}/languages/${languageCode}`);
  }

  getStaticContentLanguages(key: string) {
    return this.#http.get<Array<StaticContentLanguageDto>>(`${this.#apiUrlRoot}${key}/languages`);
  }

  createStaticContentLanguage(key: string, languageCode: string, createDto: CreateStaticContentLanguageDto) {
    return this.#http.post<StaticContentLanguageDto>(`${this.#apiUrlRoot}${key}/languages/${languageCode}`, createDto);
  }

  updateStaticContentLanguage(key: string, languageCode: string, updateDto: UpdateStaticContentLanguageDto) {
    return this.#http.put<StaticContentLanguageDto>(`${this.#apiUrlRoot}${key}/languages/${languageCode}`, updateDto);
  }

  deleteStaticContentLanguage(key: string, languageCode: string) {
    return this.#http.delete<boolean>(`${this.#apiUrlRoot}${key}/languages/${languageCode}`);
  }
}
