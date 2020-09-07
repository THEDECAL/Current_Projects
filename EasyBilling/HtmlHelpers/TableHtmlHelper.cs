﻿using EasyBilling.Helpers;
using EasyBilling.Models;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EasyBilling.HtmlHelpers
{
    public class TableHtmlHelper<T> where T : class
    {
        const int PAGE_COUNT = 3; //Кол-во страниц на секцию
        private Type _type;
        private DataViewModel<T> _data;
        public TableHtmlHelper(DataViewModel<T> data)
        {
            _type = typeof(T);
            _data = data;
        }
        /// <summary>
        /// Получить Html код заголовоки столбцов таблицы
        /// </summary>
        /// <returns></returns>
        public async Task<HtmlString> GetTableHeadAsync(bool isShowActions = true)
        {
            return await Task.Run(async () =>
            {
                TagBuilder thead = new TagBuilder("thead");
                TagBuilder tr = new TagBuilder("tr");
                tr.AddCssClass("table-dark");
                thead.InnerHtml.AppendHtml(tr);

                var props = _type.GetProperties();
                foreach (var item in props)
                {
                    if (!_data.ExcludeFields.Any(f => f.Equals(item.Name)))
                    {
                        var dnAtt = item.GetCustomAttribute(
                            typeof(DisplayNameAttribute)) as DisplayNameAttribute;

                        TagBuilder th = new TagBuilder("th");
                        th.Attributes.Add("scope", "col");

                        TagBuilder aFieldLink = new TagBuilder("a");
                        aFieldLink.AddCssClass("text-white btn btn-link btn-sm sort-field");
                        aFieldLink.Attributes.Add("href",$"/{_data.ControllerName}");
                        aFieldLink.Attributes.Add("value", item.Name);
                        aFieldLink.InnerHtml.Append((dnAtt != null) ? dnAtt.DisplayName : item.Name);
                        th.InnerHtml.AppendHtml(aFieldLink);
                        tr.InnerHtml.AppendHtml(th);
                    }
                }

                if (isShowActions)
                {
                    TagBuilder th = new TagBuilder("th");
                    th.Attributes.Add("scope", "col");

                    TagBuilder aAct = new TagBuilder("a");
                    aAct.AddCssClass("text-white btn btn-link btn-sm sort-field");
                    aAct.InnerHtml.Append("Действия");
                    th.InnerHtml.AppendHtml(aAct);
                    tr.InnerHtml.AppendHtml(th);
                }

                return await GetHtmlStringAsync(thead);
            });
        }
        /// <summary>
        /// Получить Html код панели постраничной навигации
        /// </summary>
        /// <returns></returns>
        public async Task<HtmlString> GetPaginationPanelAsync()
        {
            return await Task.Run(async () =>
            {
                TagBuilder nav = new TagBuilder("nav");
                TagBuilder ul = new TagBuilder("ul");

                ul.AddCssClass("pagination form-inline row justify-content-center");
                nav.InnerHtml.AppendHtml(ul);

                TagBuilder liPrev = new TagBuilder("li");
                liPrev.Attributes.Add("class", $"page-item {(_data.IsHavePreviousPage ? "" : "disabled")}");
                var textPrevious = "Предыдущая";
                if (!_data.IsHavePreviousPage)
                {
                    TagBuilder span = new TagBuilder("span");
                    span.AddCssClass("page-link");
                    span.InnerHtml.Append(textPrevious);
                    liPrev.InnerHtml.AppendHtml(span);
                }
                else
                {
                    TagBuilder aPrev = new TagBuilder("a");
                    var valPrev = (_data.Settings.CurrentPage - 1).ToString();
                    aPrev.AddCssClass("page-link page-control-panel");
                    aPrev.Attributes.Add("href", $"/{_data.ControllerName}");
                    //aPrev.Attributes.Add("type", "submit");
                    //btnPrev.Attributes.Add("formaction", '/' + _data.ControllerName);
                    //aPrev.Attributes.Add("name", "Pagination" + valPrev);
                    aPrev.Attributes.Add("value", valPrev);
                    aPrev.InnerHtml.Append(textPrevious);
                    liPrev.InnerHtml.AppendHtml(aPrev);
                }

                TagBuilder liNext = new TagBuilder("li");
                liNext.Attributes.Add("class", $"page-item {(_data.IsHaveNextPage ? "" : "disabled")}");
                var textNext = "Следующая";
                if (!_data.IsHaveNextPage)
                {
                    TagBuilder span = new TagBuilder("span");
                    span.Attributes.Add("class", "page-link");
                    span.InnerHtml.Append(textNext);
                    liNext.InnerHtml.AppendHtml(span);
                }
                else
                {
                    TagBuilder aNext = new TagBuilder("a");
                    var valNext = (_data.Settings.CurrentPage + 1).ToString();
                    aNext.AddCssClass("page-link page-control-panel");
                    aNext.Attributes.Add("href",$"/{_data.ControllerName}");
                    //aNext.Attributes.Add("type", "submit");
                    //aNext.Attributes.Add("name", "Pagination" + valNext);
                    //btnNext.Attributes.Add("formaction", '/' + _data.ControllerName);
                    aNext.Attributes.Add("value", valNext);
                    aNext.InnerHtml.Append(textNext);
                    liNext.InnerHtml.AppendHtml(aNext);
                }

                ul.InnerHtml.AppendHtml(liPrev);

                int currSection = (_data.Settings.CurrentPage - 1) / PAGE_COUNT; //Текущая секция страниц (с ноля)
                for (int i = 1; i <= PAGE_COUNT; i++)
                {
                    int pageNum = i + (currSection * PAGE_COUNT);
                    if (pageNum <= _data.AmountPage) //Если текущая страница входит в общее кол-во страниц
                    {
                        TagBuilder li = new TagBuilder("li");
                        li.Attributes.Add("class", $"page-item {(_data.Settings.CurrentPage == pageNum ? "active" : "")}");
                        ul.InnerHtml.AppendHtml(li);

                        if (_data.Settings.CurrentPage == pageNum)
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.Attributes.Add("class", "page-link");
                            span.InnerHtml.Append(pageNum.ToString());
                            li.InnerHtml.AppendHtml(span);
                        }
                        else
                        {
                            TagBuilder aPageNum = new TagBuilder("a");
                            aPageNum.AddCssClass("page-link page-control-panel");
                            aPageNum.Attributes.Add("href", $"/{_data.ControllerName}");
                            //btn.Attributes.Add("type", "submit");
                            //btn.Attributes.Add("name", pageNum.ToString());
                            aPageNum.Attributes.Add("value", pageNum.ToString());
                            aPageNum.InnerHtml.Append(pageNum.ToString());
                            li.InnerHtml.AppendHtml(aPageNum);
                        }
                    }
                    else break;
                }

                ul.InnerHtml.AppendHtml(liNext);

                return await GetHtmlStringAsync(nav);
            });
        }
        /// <summary>
        /// Получить Html код панели поиска, добавления...
        /// </summary>
        /// <returns></returns>
        public async Task<HtmlString> GetControlPanelAsync(bool isShowBtnAdd = true)
        {
            return await Task.Run(async () =>
            {
                TagBuilder div = new TagBuilder("div");
                div.AddCssClass("form-inline row justify-content-center");
                div.Attributes.Add("method", "get");
                div.Attributes.Add("style", "width: 1000px; margin: auto; margin-bottom: 10px;");

                if (isShowBtnAdd)
                {
                    TagBuilder a = new TagBuilder("a");
                    a.AddCssClass("btn btn-raised btn-success mt-5 mr-3");
                    a.Attributes.Add("href", $"/{_data.ControllerName}/AddUpdateForm");
                    a.InnerHtml.Append("Добавить");
                    div.InnerHtml.AppendHtml(a);
                }

                TagBuilder fgrp = new TagBuilder("div");
                fgrp.AddCssClass("form-group");
                div.InnerHtml.AppendHtml(fgrp);

                TagBuilder input = new TagBuilder("input");
                input.AddCssClass("form-control");
                input.Attributes.Add("type", "text");
                input.Attributes.Add("id", nameof(ControlPanelSettings.SearchText));
                input.Attributes.Add("placeholder", "Поиск по всем полям");
                input.Attributes.Add("style", "width: 500px");
                input.Attributes.Add("value", _data.Settings.SearchText);
                fgrp.InnerHtml.AppendHtml(input);

                TagBuilder btn = new TagBuilder("button");
                btn.AddCssClass("btn btn-primary mt-5");
                btn.Attributes.Add("type", "submit");
                btn.Attributes.Add("id", "StartSearch");
                btn.InnerHtml.Append("Поиск");
                div.InnerHtml.AppendHtml(btn);

                div.InnerHtml.AppendFormat($"<input type='hidden' id='{nameof(ControlPanelSettings.PageRowsCount)}' value='{_data.Settings.PageRowsCount}' />");
                div.InnerHtml.AppendFormat($"<input type='hidden' id='{nameof(ControlPanelSettings.SortType)}' value='{(int)_data.Settings.SortType}' />");

                return await GetHtmlStringAsync(div);
            });
        }
        private async Task<HtmlString> GetHtmlStringAsync(TagBuilder builder)
        {
            return await Task.Run(() =>
            {
                //builder.InnerHtml.AppendFormat(@"<script src='/js/cpanel_settings.js' type='text/javascript'></script>");
                var writer = new StringWriter();
                builder.WriteTo(writer, HtmlEncoder.Default);
                return new HtmlString(writer.ToString());
            });
        }
        private string GetHref(string sort, SortType sortType, int page, int pageSize)
            => $"/{_data.ControllerName}?SortField={sort}&SorType={(int)sortType}&CurrentPage={page}&PageRowsCount={pageSize}&SearchText={_data.Settings.SearchText}";
    }
}
