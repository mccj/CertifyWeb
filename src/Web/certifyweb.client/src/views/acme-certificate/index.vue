<template>
    <div>
        <el-button type="primary" @click="openAddDomainOrder"> 新增 </el-button>
        <el-table :data="state.data" style="width: 100%" v-loading="state.loading" border>
            <el-table-column type="index" label="序号" width="55" align="center" fixed />
            <el-table-column prop="acmeTitle" label="证书颁发机构" align="center" show-overflow-tooltip />
            <el-table-column prop="domains" label="域名" align="center" show-overflow-tooltip />
            <el-table-column prop="orderExpires" label="有效期" align="center" show-overflow-tooltip />
            <el-table-column prop="orderStatus" label="状态" align="center" show-overflow-tooltip>
                <template #default="scope">
                    <el-tag type="success" v-if="scope.row.orderStatus === 'valid'">已签发</el-tag>
                    <el-tag type="primary" v-else-if="scope.row.orderStatus === 'pending'">待申请</el-tag>
                    <el-tag type="info" v-else>{{ scope.row.orderStatus }}</el-tag>

                </template>
            </el-table-column>
            <el-table-column prop="description" label="描述" align="center" show-overflow-tooltip />
            <el-table-column label="操作" width="240" fixed="right" align="center" show-overflow-tooltip>
                <template #default="scope">
                    <el-button v-if="scope.row.orderStatus !== 'valid'" size="small" text
                        @click="editRequestDomainRef?.openDialog(scope.row)">申请新证书</el-button>
                    <el-button v-if="scope.row.orderStatus === 'valid'" size="small" text
                        @click="editCertificateInfoRef?.openDialog(scope.row)">下载证书</el-button>
                    <!-- <el-button v-if="scope.row.orderStatus === 'valid'" size="small" text type="danger"
                        @click="revokeCert(scope.row)"> 吊销证书 </el-button> -->
                    <el-button v-if="scope.row.orderStatus !== 'valid'" size="small" text type="danger"
                        @click="delDns(scope.row)"> 删除 </el-button>
                </template>
            </el-table-column>

        </el-table>
        <el-pagination v-model:currentPage="state.tableParams.page" v-model:page-size="state.tableParams.pageSize"
            :total="state.tableParams.total" :page-sizes="[10, 20, 50, 100]" size="small" background
            @size-change="handleSizeChange" @current-change="handleCurrentChange"
            layout="total, sizes, prev, pager, next, jumper" />
        <DomainOrderAdd ref="domainOrderAddRef" @handleQuery="handleQuery" />
        <RequestDomain ref="editRequestDomainRef" />
        <CertificateInfo ref="editCertificateInfoRef" />
    </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessageBox, ElMessage } from 'element-plus';
import client, { DomainOrderDetailOutput } from '@/api';
import DomainOrderAdd from './components/domain-order-add.vue'
import RequestDomain from './components/request-domain.vue'
import CertificateInfo from './components/certificate-info.vue'
const domainOrderAddRef = ref<InstanceType<typeof DomainOrderAdd>>();
const editRequestDomainRef = ref<InstanceType<typeof RequestDomain>>();
const editCertificateInfoRef = ref<InstanceType<typeof CertificateInfo>>();

const state = reactive({
    loading: false,
    data: [] as Array<DomainOrderDetailOutput>,
    //queryParams: {
    //  name: undefined,
    //  code: undefined,
    //},
    tableParams: {
        page: 1,
        pageSize: 50,
        total: 0,
    }
    //editRoleTitle: '',
});


onMounted(async () => {
    await handleQuery();
});

// 查询操作
const handleQuery = async () => {
    state.loading = true;
    //let params = Object.assign(state.queryParams, state.tableParams);
    const res = await client.ApiDomainOrderGetPage({});
    state.data = res?.items ?? [];
    state.tableParams.total = res?.total ?? 0;
    state.loading = false;
};

// 打开新增页面
const openAddDomainOrder = () => {
    domainOrderAddRef.value?.openDialog({});
};

// 打开编辑页面
const openEditDns = async (row: unknown) => {
    domainOrderAddRef.value?.openDialog(row);
};
// 吊销证书
const revokeCert = (row: DomainOrderDetailOutput) => {
    ElMessageBox.confirm(`确定吊销证书吗：【${row.domains}】?`, '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
    })
        .then(async () => {
            await client.ApiRequestDomainRevokeCert(row.id);
            await handleQuery();
            ElMessage.success('删除成功');
        })
        .catch(() => { });
};

// 删除
const delDns = (row: DomainOrderDetailOutput) => {
    ElMessageBox.confirm(`确定删记录：【${row.domains}】?`, '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
    })
        .then(async () => {
            await client.ApiDomainOrderDelete(row.id);
            await handleQuery();
            ElMessage.success('删除成功');
        })
        .catch(() => { });
};

// 改变页面容量
const handleSizeChange = async (val: number) => {
    state.tableParams.pageSize = val;
    await handleQuery();
};

// 改变页码序号
const handleCurrentChange = async (val: number) => {
    state.tableParams.page = val;
    await handleQuery();
};

</script>

<style lang="scss" scoped></style>
