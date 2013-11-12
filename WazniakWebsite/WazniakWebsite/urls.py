from django.conf.urls import patterns, include, url
from wazniakWeb.views import SampleItemListView

# Uncomment the next two lines to enable the admin:
from django.contrib import admin
from wazniakWeb.views import ItemDeleteView, ItemCreateView, LatexView
admin.autodiscover()

urlpatterns = patterns('',
    # Examples:
    url(r'^$', SampleItemListView.as_view(), name='home'),
    # url(r'^WazniakWebsite/', include('WazniakWebsite.foo.urls')),

    # Uncomment the admin/doc line below to enable admin documentation:
    url(r'^admin/doc/', include('django.contrib.admindocs.urls')),

    # Uncomment the next line to enable the admin:
    url(r'^admin/', include(admin.site.urls)),
    url(r'^addItem/', ItemCreateView.as_view(), name='add_item'),
    url(r'^item/(?P<pk>\d+)/delete/$', ItemDeleteView.as_view(), name='delete_item'),


    url(r'^latex/', LatexView.as_view(), name='latex'),
)
